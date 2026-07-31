using Backend.DTOs;

namespace Backend.Services;

public interface ISaleService
{
    Task<IReadOnlyList<SaleResponse>> GetAllAsync(
        int take,
        CancellationToken cancellationToken);

    Task<SaleResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<SaleResponse> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken);
}
