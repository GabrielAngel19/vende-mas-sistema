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
        int? storeId,
        int? branchId,
        bool availableOnly,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking().AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(product => product.IsActive);
        }

        if (storeId.HasValue)
        {
            query = query.Where(product => product.StoreId == storeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product =>
                product.Name.Contains(term)
                || product.Category.Contains(term)
                || (product.Brand != null && product.Brand.Contains(term))
                || (product.Code != null && product.Code.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            query = query.Where(product => product.Category == normalizedCategory);
        }

        if (branchId.HasValue)
        {
            query = query.Where(product => product.Inventories.Any(inventory =>
                inventory.BranchId == branchId.Value));
        }

        if (availableOnly)
        {
            query = branchId.HasValue
                ? query.Where(product => product.Inventories.Any(inventory =>
                    inventory.BranchId == branchId.Value
                    && inventory.Quantity > 0))
                : query.Where(product =>
                    product.Stock > 0
                    || product.Inventories.Any(inventory => inventory.Quantity > 0));
        }

        return await query
            .OrderBy(product => product.Name)
            .Select(product => new ProductResponse(
                product.Id,
                product.StoreId,
                product.Store != null ? product.Store.Name : null,
                product.Name,
                product.Category,
                product.Brand,
                product.Description,
                product.Code,
                product.ImageUrl,
                product.Price,
                branchId.HasValue
                    ? (product.Inventories
                        .Where(inventory => inventory.BranchId == branchId.Value)
                        .Sum(inventory => (int?)inventory.Quantity) ?? 0)
                    : product.Stock,
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
            .Include(item => item.Store)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el producto con id {id}.");

        return ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var store = await dbContext.Stores
            .SingleOrDefaultAsync(item =>
                item.Id == request.StoreId && item.IsActive,
                cancellationToken)
            ?? throw new NotFoundException(
                $"No existe una tienda activa con id {request.StoreId}.");

        var branch = await dbContext.Branches
            .SingleOrDefaultAsync(item =>
                item.Id == request.BranchId
                && item.StoreId == request.StoreId
                && item.IsActive,
                cancellationToken)
            ?? throw new NotFoundException(
                "La sucursal no existe o no pertenece a la tienda.");

        var code = Normalize(request.Code);
        if (code is not null && await dbContext.Products.AnyAsync(
            product => product.StoreId == store.Id && product.Code == code,
            cancellationToken))
        {
            throw new ConflictException(
                $"Ya existe un producto con el código '{code}' en la tienda.");
        }

        var category = await GetOrCreateCategoryAsync(
            store,
            request.Category,
            cancellationToken);

        var product = new Product
        {
            Store = store,
            CategoryEntity = category,
            Name = request.Name.Trim(),
            Category = category.Name,
            Brand = Normalize(request.Brand),
            Description = Normalize(request.Description),
            Code = code,
            ImageUrl = Normalize(request.ImageUrl),
            Price = request.Price,
            Stock = request.Stock,
            Unit = request.Unit.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Products.Add(product);
        dbContext.Inventories.Add(new Inventory
        {
            Branch = branch,
            Product = product,
            Quantity = request.Stock,
            ReorderLevel = 5,
            UpdatedAtUtc = product.CreatedAtUtc
        });

        if (product.Stock > 0)
        {
            dbContext.InventoryMovements.Add(new InventoryMovement
            {
                Product = product,
                Branch = branch,
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
            .Include(item => item.Store)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el producto con id {id}.");

        if (product.Store is null)
        {
            throw new ConflictException(
                "El producto aún no ha sido asignado a una tienda.");
        }

        var code = Normalize(request.Code);
        if (code is not null && await dbContext.Products.AnyAsync(
            item => item.Id != id
                && item.StoreId == product.StoreId
                && item.Code == code,
            cancellationToken))
        {
            throw new ConflictException(
                $"Ya existe un producto con el código '{code}' en la tienda.");
        }

        var category = await GetOrCreateCategoryAsync(
            product.Store,
            request.Category,
            cancellationToken);

        product.Name = request.Name.Trim();
        product.CategoryEntity = category;
        product.Category = category.Name;
        product.Brand = Normalize(request.Brand);
        product.Description = Normalize(request.Description);
        product.Code = code;
        product.ImageUrl = Normalize(request.ImageUrl);
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

    private async Task<Category> GetOrCreateCategoryAsync(
        Store store,
        string name,
        CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim();
        var category = await dbContext.Categories.SingleOrDefaultAsync(
            item => item.StoreId == store.Id && item.Name == normalizedName,
            cancellationToken);

        if (category is not null)
        {
            return category;
        }

        category = new Category
        {
            Store = store,
            Name = normalizedName,
            Slug = ToSlug(normalizedName)
        };
        dbContext.Categories.Add(category);
        return category;
    }

    private static ProductResponse ToResponse(Product product) =>
        new(
            product.Id,
            product.StoreId,
            product.Store?.Name,
            product.Name,
            product.Category,
            product.Brand,
            product.Description,
            product.Code,
            product.ImageUrl,
            product.Price,
            product.Stock,
            product.Unit,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);

    private static string ToSlug(string value)
    {
        var chars = value.ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray();
        return string.Join('-', new string(chars)
            .Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
