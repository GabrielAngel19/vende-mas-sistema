using System.Data;
using Backend.Data;
using Backend.DTOs;
using Backend.Middleware;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public sealed class InventoryService(AppDbContext dbContext) : IInventoryService
{
    public async Task<IReadOnlyList<ProductInventoryResponse>> GetInventoryAsync(
        int storeId,
        int? branchId,
        int lowStockThreshold,
        CancellationToken cancellationToken)
    {
        var threshold = Math.Max(0, lowStockThreshold);
        var query = dbContext.Products
            .AsNoTracking()
            .Where(product => product.StoreId == storeId && product.IsActive);

        return await query
            .OrderBy(product => product.Name)
            .Select(product => new ProductInventoryResponse(
                product.Id,
                product.StoreId,
                branchId,
                product.Name,
                product.Category,
                branchId.HasValue
                    ? (product.Inventories
                        .Where(inventory => inventory.BranchId == branchId.Value)
                        .Sum(inventory => (int?)inventory.Quantity) ?? 0)
                    : product.Stock,
                product.Unit,
                (branchId.HasValue
                    ? (product.Inventories
                        .Where(inventory => inventory.BranchId == branchId.Value)
                        .Sum(inventory => (int?)inventory.Quantity) ?? 0)
                    : product.Stock) <= threshold))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductInventoryResponse>> GetLowStockAsync(
        int storeId,
        int? branchId,
        int threshold,
        CancellationToken cancellationToken)
    {
        var inventory = await GetInventoryAsync(
            storeId,
            branchId,
            threshold,
            cancellationToken);

        return inventory
            .Where(item => item.IsLowStock)
            .OrderBy(item => item.Stock)
            .ThenBy(item => item.ProductName)
            .ToList();
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(
        int storeId,
        int? branchId,
        int? productId,
        int take,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(take, 1, 200);
        var query = dbContext.InventoryMovements
            .AsNoTracking()
            .Where(movement => movement.Product.StoreId == storeId);

        if (branchId.HasValue)
        {
            query = query.Where(movement => movement.BranchId == branchId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(movement => movement.ProductId == productId.Value);
        }

        return await query
            .OrderByDescending(movement => movement.CreatedAtUtc)
            .Take(limit)
            .Select(movement => new InventoryMovementResponse(
                movement.Id,
                movement.ProductId,
                movement.BranchId,
                movement.Product.Name,
                movement.SaleId,
                movement.Type,
                movement.QuantityChange,
                movement.PreviousStock,
                movement.NewStock,
                movement.Reason,
                movement.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductInventoryResponse> AdjustAsync(
        int productId,
        InventoryAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.QuantityChange == 0)
        {
            throw new BusinessRuleException(
                "El ajuste de inventario no puede ser cero.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var product = await dbContext.Products
            .Include(item => item.Inventories)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken)
            ?? throw new NotFoundException(
                $"No existe el producto con id {productId}.");

        if (!product.StoreId.HasValue)
        {
            throw new ConflictException(
                "El producto aún no ha sido asignado a una tienda.");
        }

        var branchId = request.BranchId
            ?? await dbContext.Branches
                .Where(branch =>
                    branch.StoreId == product.StoreId
                    && branch.IsPrimary
                    && branch.IsActive)
                .Select(branch => (int?)branch.Id)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ConflictException(
                "La tienda no tiene una sucursal principal activa.");

        var branchBelongsToStore = await dbContext.Branches.AnyAsync(
            branch =>
                branch.Id == branchId
                && branch.StoreId == product.StoreId
                && branch.IsActive,
            cancellationToken);

        if (!branchBelongsToStore)
        {
            throw new ConflictException(
                "La sucursal no pertenece a la tienda del producto.");
        }

        var inventory = product.Inventories.SingleOrDefault(item =>
            item.BranchId == branchId);

        if (inventory is null)
        {
            inventory = new Inventory
            {
                Product = product,
                BranchId = branchId,
                Quantity = 0,
                ReorderLevel = 5,
                UpdatedAtUtc = DateTime.UtcNow
            };
            dbContext.Inventories.Add(inventory);
        }

        var previousStock = inventory.Quantity;
        var newStock = previousStock + request.QuantityChange;

        if (newStock < 0)
        {
            throw new ConflictException(
                $"El ajuste dejaría el inventario en {newStock}. "
                + $"Existencia actual: {previousStock}.");
        }

        inventory.Quantity = newStock;
        inventory.UpdatedAtUtc = DateTime.UtcNow;
        product.Stock += request.QuantityChange;
        product.UpdatedAtUtc = DateTime.UtcNow;

        dbContext.InventoryMovements.Add(new InventoryMovement
        {
            ProductId = product.Id,
            BranchId = branchId,
            Type = request.QuantityChange > 0 ? "Entrada" : "Salida",
            QuantityChange = request.QuantityChange,
            PreviousStock = previousStock,
            NewStock = newStock,
            Reason = request.Reason.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ProductInventoryResponse(
            product.Id,
            product.StoreId,
            branchId,
            product.Name,
            product.Category,
            inventory.Quantity,
            product.Unit,
            inventory.Quantity <= inventory.ReorderLevel);
    }
}
