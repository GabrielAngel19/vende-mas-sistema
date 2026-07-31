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
        int lowStockThreshold,
        CancellationToken cancellationToken)
    {
        var threshold = Math.Max(0, lowStockThreshold);

        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.Name)
            .Select(product => new ProductInventoryResponse(
                product.Id,
                product.Name,
                product.Category,
                product.Stock,
                product.Unit,
                product.Stock <= threshold))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductInventoryResponse>> GetLowStockAsync(
        int threshold,
        CancellationToken cancellationToken)
    {
        var normalizedThreshold = Math.Max(0, threshold);

        return await dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive && product.Stock <= normalizedThreshold)
            .OrderBy(product => product.Stock)
            .ThenBy(product => product.Name)
            .Select(product => new ProductInventoryResponse(
                product.Id,
                product.Name,
                product.Category,
                product.Stock,
                product.Unit,
                true))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(
        int? productId,
        int take,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(take, 1, 200);
        var query = dbContext.InventoryMovements.AsNoTracking().AsQueryable();

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
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken)
            ?? throw new NotFoundException(
                $"No existe el producto con id {productId}.");

        var previousStock = product.Stock;
        var newStock = previousStock + request.QuantityChange;

        if (newStock < 0)
        {
            throw new ConflictException(
                $"El ajuste dejaría el inventario en {newStock}. "
                + $"Existencia actual: {previousStock}.");
        }

        product.Stock = newStock;
        product.UpdatedAtUtc = DateTime.UtcNow;

        dbContext.InventoryMovements.Add(new InventoryMovement
        {
            ProductId = product.Id,
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
            product.Name,
            product.Category,
            product.Stock,
            product.Unit,
            product.Stock <= 5);
    }
}
