using Backend.DTOs;

namespace Backend.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> GetAllAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken);

    Task<CustomerResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken);

    Task<CustomerResponse> UpdateAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
