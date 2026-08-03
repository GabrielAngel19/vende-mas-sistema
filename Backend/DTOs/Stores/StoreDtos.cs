using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Stores;

public sealed class CreateBranchRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(300, MinimumLength = 3)]
    public string Address { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string City { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string State { get; init; } = string.Empty;

    [Range(-90, 90)]
    public decimal? Latitude { get; init; }

    [Range(-180, 180)]
    public decimal? Longitude { get; init; }
}

public sealed class CreateStoreUserRequest
{
    [Required, EmailAddress, StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}

public sealed record BranchResponse(
    int Id,
    int StoreId,
    string Name,
    string Address,
    string City,
    string State,
    decimal? Latitude,
    decimal? Longitude,
    bool IsPrimary,
    bool IsActive);

public sealed record StoreSummaryResponse(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoUrl,
    string? City,
    string? State,
    int ProductCount);

public sealed record StoreDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoUrl,
    IReadOnlyList<BranchResponse> Branches);

public sealed record StoreUserResponse(
    int UserId,
    int StoreId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive);
