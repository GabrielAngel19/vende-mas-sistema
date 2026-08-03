using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Auth;

public class RegisterCustomerRequest
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

public sealed class RegisterStoreOwnerRequest : RegisterCustomerRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string StoreName { get; init; } = string.Empty;

    [Required, RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    [StringLength(160, MinimumLength = 2)]
    public string StoreSlug { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? StoreDescription { get; init; }

    [Url, StringLength(500)]
    public string? LogoUrl { get; init; }

    [Required, StringLength(150, MinimumLength = 2)]
    public string BranchName { get; init; } = string.Empty;

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

public sealed class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record StoreMembershipResponse(
    int StoreId,
    string StoreName,
    string StoreSlug,
    string Role);

public sealed record AuthUserResponse(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<StoreMembershipResponse> Stores);

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    AuthUserResponse User);
