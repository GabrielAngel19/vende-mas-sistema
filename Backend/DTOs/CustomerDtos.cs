using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public sealed class CreateCustomerRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [EmailAddress, StringLength(180)]
    public string? Email { get; init; }

    [Phone, StringLength(30)]
    public string? Phone { get; init; }

    [StringLength(20)]
    public string? TaxId { get; init; }
}

public sealed class UpdateCustomerRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [EmailAddress, StringLength(180)]
    public string? Email { get; init; }

    [Phone, StringLength(30)]
    public string? Phone { get; init; }

    [StringLength(20)]
    public string? TaxId { get; init; }

    public bool IsActive { get; init; } = true;
}

public sealed record CustomerResponse(
    int Id,
    string Name,
    string? Email,
    string? Phone,
    string? TaxId,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
