using System.Data;
using Backend.Data;
using Backend.DTOs;
using Backend.Middleware;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public sealed class SaleService(AppDbContext dbContext) : ISaleService
{
    public async Task<IReadOnlyList<SaleResponse>> GetAllAsync(
        int take,
        CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(take, 1, 200);
        var sales = await BaseQuery()
            .Take(limit)
            .ToListAsync(cancellationToken);

        return sales.Select(ToResponse).ToList();
    }

    public async Task<SaleResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var sale = await BaseQuery()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe la venta con id {id}.");

        return ToResponse(sale);
    }

    public async Task<SaleResponse> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var groupedItems = request.Items
            .GroupBy(item => item.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        Customer? customer = null;
        if (request.CustomerId.HasValue)
        {
            customer = await dbContext.Customers.SingleOrDefaultAsync(
                item => item.Id == request.CustomerId.Value && item.IsActive,
                cancellationToken)
                ?? throw new NotFoundException(
                    $"No existe un cliente activo con id {request.CustomerId}.");
        }

        var productIds = groupedItems.Select(item => item.ProductId).ToList();
        var products = await dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, cancellationToken);

        var missingIds = productIds
            .Where(productId => !products.ContainsKey(productId))
            .ToArray();

        if (missingIds.Length > 0)
        {
            throw new NotFoundException(
                $"No existen los productos: {string.Join(", ", missingIds)}.");
        }

        var inactiveProducts = products.Values
            .Where(product => !product.IsActive)
            .Select(product => product.Name)
            .ToArray();

        if (inactiveProducts.Length > 0)
        {
            throw new ConflictException(
                $"Los siguientes productos están inactivos: "
                + string.Join(", ", inactiveProducts));
        }

        foreach (var item in groupedItems)
        {
            var product = products[item.ProductId];
            if (product.Stock < item.Quantity)
            {
                throw new ConflictException(
                    $"Inventario insuficiente para {product.Name}. "
                    + $"Disponible: {product.Stock}; solicitado: {item.Quantity}.");
            }
        }

        var createdAt = DateTime.UtcNow;
        var sale = new Sale
        {
            Customer = customer,
            PaymentMethod = request.PaymentMethod.Trim(),
            Status = "Completada",
            CreatedAtUtc = createdAt
        };

        foreach (var item in groupedItems)
        {
            var product = products[item.ProductId];
            var previousStock = product.Stock;
            var lineTotal = decimal.Round(
                product.Price * item.Quantity,
                2,
                MidpointRounding.AwayFromZero);

            product.Stock -= item.Quantity;
            product.UpdatedAtUtc = createdAt;

            sale.Items.Add(new SaleItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                LineTotal = lineTotal
            });

            dbContext.InventoryMovements.Add(new InventoryMovement
            {
                ProductId = product.Id,
                Sale = sale,
                Type = "Venta",
                QuantityChange = -item.Quantity,
                PreviousStock = previousStock,
                NewStock = product.Stock,
                Reason = "Salida por venta",
                CreatedAtUtc = createdAt
            });
        }

        sale.Subtotal = sale.Items.Sum(item => item.LineTotal);
        sale.Tax = decimal.Round(
            sale.Subtotal * request.TaxRate,
            2,
            MidpointRounding.AwayFromZero);
        sale.Total = sale.Subtotal + sale.Tax;

        dbContext.Sales.Add(sale);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetByIdAsync(sale.Id, cancellationToken);
    }

    private IQueryable<Sale> BaseQuery() =>
        dbContext.Sales
            .AsNoTracking()
            .Include(sale => sale.Customer)
            .Include(sale => sale.Items)
            .OrderByDescending(sale => sale.CreatedAtUtc);

    private static SaleResponse ToResponse(Sale sale) =>
        new(
            sale.Id,
            sale.CustomerId,
            sale.Customer?.Name,
            sale.PaymentMethod,
            sale.Subtotal,
            sale.Tax,
            sale.Total,
            sale.Status,
            sale.CreatedAtUtc,
            sale.Items
                .OrderBy(item => item.Id)
                .Select(item => new SaleItemResponse(
                    item.ProductId,
                    item.ProductName,
                    item.UnitPrice,
                    item.Quantity,
                    item.LineTotal))
                .ToList());
}
