using System.Globalization;
using System.Text;
using Backend.Data;
using Backend.DTOs.Catalog;
using Backend.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Catalog;

public sealed class CatalogService(AppDbContext dbContext) : ICatalogService
{
    public async Task<IReadOnlyList<CatalogProductResponse>> GetAllAsync(
        string? search,
        string? category,
        int? storeId,
        string? city,
        bool availableOnly,
        CancellationToken cancellationToken)
    {
        var rows = await LoadRowsAsync(
            search,
            category,
            storeId,
            city,
            availableOnly,
            cancellationToken);

        return GroupRows(rows);
    }

    public async Task<CatalogProductResponse> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var rows = await LoadRowsAsync(
            search: null,
            category: null,
            storeId: null,
            city: null,
            availableOnly: false,
            cancellationToken: cancellationToken);

        var product = GroupRows(rows)
            .SingleOrDefault(item => item.Offers.Any(offer =>
                offer.ProductId == productId));

        return product ?? throw new NotFoundException(
            $"No existe un producto activo con id {productId} en el catálogo.");
    }

    private async Task<List<CatalogRow>> LoadRowsAsync(
        string? search,
        string? category,
        int? storeId,
        string? city,
        bool availableOnly,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive
                && product.StoreId.HasValue
                && product.Store != null
                && product.Store.IsActive);

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
            query = query.Where(product =>
                product.Category == normalizedCategory);
        }

        if (storeId.HasValue)
        {
            query = query.Where(product => product.StoreId == storeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim();
            query = query.Where(product => product.Store!.Branches.Any(branch =>
                branch.IsActive && branch.City == normalizedCity));
        }

        if (availableOnly)
        {
            query = query.Where(product =>
                product.Stock > 0
                || product.Inventories.Any(inventory => inventory.Quantity > 0));
        }

        return await query
            .Select(product => new CatalogRow(
                product.Id,
                product.StoreId!.Value,
                product.Store!.Name,
                product.Store.Slug,
                product.Store.Branches
                    .Where(branch => branch.IsActive && branch.IsPrimary)
                    .Select(branch => branch.City)
                    .FirstOrDefault(),
                product.Store.Branches
                    .Where(branch => branch.IsActive && branch.IsPrimary)
                    .Select(branch => branch.State)
                    .FirstOrDefault(),
                product.Name,
                product.Category,
                product.Brand,
                product.Description,
                product.Code,
                product.ImageUrl,
                product.Price,
                product.Inventories.Any()
                    ? product.Inventories.Sum(inventory => inventory.Quantity)
                    : product.Stock,
                product.Unit))
            .ToListAsync(cancellationToken);
    }

    private static IReadOnlyList<CatalogProductResponse> GroupRows(
        IEnumerable<CatalogRow> rows)
    {
        return rows
            .GroupBy(row => CreateCatalogKey(
                row.Code,
                row.Name,
                row.Brand,
                row.Category,
                row.Unit))
            .Select(group =>
            {
                var first = group
                    .OrderBy(row => row.Price)
                    .ThenBy(row => row.ProductId)
                    .First();
                var offers = group
                    .OrderBy(row => row.Price)
                    .ThenBy(row => row.StoreName)
                    .Select(row => new CatalogOfferResponse(
                        row.ProductId,
                        row.StoreId,
                        row.StoreName,
                        row.StoreSlug,
                        row.City,
                        row.State,
                        row.Price,
                        row.Stock,
                        row.Unit,
                        row.Stock > 0))
                    .ToList();

                return new CatalogProductResponse(
                    group.Key,
                    first.ProductId,
                    first.Name,
                    first.Category,
                    first.Brand,
                    first.Description,
                    first.Code,
                    first.ImageUrl,
                    offers.Min(offer => offer.Price),
                    offers.Sum(offer => offer.Stock),
                    offers.Select(offer => offer.StoreId).Distinct().Count(),
                    offers);
            })
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Brand)
            .ToList();
    }

    private static string CreateCatalogKey(
        string? code,
        string name,
        string? brand,
        string category,
        string unit)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            return $"code:{Normalize(code)}";
        }

        return string.Join('|',
            "text",
            Normalize(name),
            Normalize(brand),
            Normalize(category),
            Normalize(unit));
    }

    private static string Normalize(string? value)
    {
        var decomposed = (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character)
                != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private sealed record CatalogRow(
        int ProductId,
        int StoreId,
        string StoreName,
        string StoreSlug,
        string? City,
        string? State,
        string Name,
        string Category,
        string? Brand,
        string? Description,
        string? Code,
        string? ImageUrl,
        decimal Price,
        int Stock,
        string Unit);
}
