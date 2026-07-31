using Backend.DTOs;

namespace Backend.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        string? search,
        string? category,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<ProductResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
