using Backend.Data;
using Backend.DTOs;
using Backend.Middleware;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public sealed class ProductService(AppDbContext dbContext) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        string? search,
        string? category,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking().AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(product => product.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product =>
                product.Name.Contains(term) || product.Category.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            query = query.Where(product => product.Category == normalizedCategory);
        }

        return await query
            .OrderBy(product => product.Name)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Category,
                product.Price,
                product.Stock,
                product.Unit,
                product.IsActive,
                product.CreatedAtUtc,
                product.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el producto con id {id}.");

        return ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            Unit = request.Unit.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Products.Add(product);

        if (product.Stock > 0)
        {
            dbContext.InventoryMovements.Add(new InventoryMovement
            {
                Product = product,
                Type = "Entrada",
                QuantityChange = product.Stock,
                PreviousStock = 0,
                NewStock = product.Stock,
                Reason = "Inventario inicial",
                CreatedAtUtc = product.CreatedAtUtc
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el producto con id {id}.");

        product.Name = request.Name.Trim();
        product.Category = request.Category.Trim();
        product.Price = request.Price;
        product.Unit = request.Unit.Trim();
        product.IsActive = request.IsActive;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el producto con id {id}.");

        product.IsActive = false;
        product.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ProductResponse ToResponse(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Category,
            product.Price,
            product.Stock,
            product.Unit,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);
}
