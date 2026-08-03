using Backend.DTOs.Catalog;

namespace Backend.Services.Catalog;

public interface ICatalogService
{
    Task<IReadOnlyList<CatalogProductResponse>> GetAllAsync(
        string? search,
        string? category,
        int? storeId,
        string? city,
        bool availableOnly,
        CancellationToken cancellationToken);

    Task<CatalogProductResponse> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken);
}
