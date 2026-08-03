using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public sealed class CreateProductRequest
{
    [Range(1, int.MaxValue)]
    public int StoreId { get; init; }

    [Range(1, int.MaxValue)]
    public int BranchId { get; init; }

    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 2)]
    public string Category { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Brand { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }

    [StringLength(100)]
    public string? Code { get; init; }

    [Url, StringLength(500)]
    public string? ImageUrl { get; init; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Price { get; init; }

    [Range(0, 1_000_000)]
    public int Stock { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public string Unit { get; init; } = "pieza";
}

public sealed class UpdateProductRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 2)]
    public string Category { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Brand { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }

    [StringLength(100)]
    public string? Code { get; init; }

    [Url, StringLength(500)]
    public string? ImageUrl { get; init; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Price { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public string Unit { get; init; } = "pieza";

    public bool IsActive { get; init; } = true;
}

public sealed record ProductResponse(
    int Id,
    int? StoreId,
    string? StoreName,
    string Name,
    string Category,
    string? Brand,
    string? Description,
    string? Code,
    string? ImageUrl,
    decimal Price,
    int Stock,
    string Unit,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
