using Backend.Data;
using Backend.DTOs;
using Backend.Middleware;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public sealed class CustomerService(AppDbContext dbContext) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Customers.AsNoTracking().AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(customer => customer.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(customer =>
                customer.Name.Contains(term)
                || (customer.Email != null && customer.Email.Contains(term))
                || (customer.Phone != null && customer.Phone.Contains(term)));
        }

        return await query
            .OrderBy(customer => customer.Name)
            .Select(customer => new CustomerResponse(
                customer.Id,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.TaxId,
                customer.IsActive,
                customer.CreatedAtUtc,
                customer.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el cliente con id {id}.");

        return ToResponse(customer);
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = Normalize(request.Email),
            Phone = Normalize(request.Phone),
            TaxId = Normalize(request.TaxId)?.ToUpperInvariant(),
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el cliente con id {id}.");

        customer.Name = request.Name.Trim();
        customer.Email = Normalize(request.Email);
        customer.Phone = Normalize(request.Phone);
        customer.TaxId = Normalize(request.TaxId)?.ToUpperInvariant();
        customer.IsActive = request.IsActive;
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(customer);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No existe el cliente con id {id}.");

        customer.IsActive = false;
        customer.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static CustomerResponse ToResponse(Customer customer) =>
        new(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.TaxId,
            customer.IsActive,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc);
}
