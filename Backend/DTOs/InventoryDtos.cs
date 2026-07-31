using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public sealed class InventoryAdjustmentRequest
{
    [Range(-1_000_000, 1_000_000)]
    public int QuantityChange { get; init; }

    [Required, StringLength(250, MinimumLength = 3)]
    public string Reason { get; init; } = string.Empty;
}

public sealed record ProductInventoryResponse(
    int ProductId,
    string ProductName,
    string Category,
    int Stock,
    string Unit,
    bool IsLowStock);

public sealed record InventoryMovementResponse(
    int Id,
    int ProductId,
    string ProductName,
    int? SaleId,
    string Type,
    int QuantityChange,
    int PreviousStock,
    int NewStock,
    string Reason,
    DateTime CreatedAtUtc);
