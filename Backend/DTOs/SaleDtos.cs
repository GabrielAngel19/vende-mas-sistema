using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public sealed class CreateSaleRequest
{
    public int? CustomerId { get; init; }

    [Required]
    [RegularExpression(
        "^(Efectivo|Tarjeta|Transferencia)$",
        ErrorMessage = "El método de pago debe ser Efectivo, Tarjeta o Transferencia.")]
    public string PaymentMethod { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "1")]
    public decimal TaxRate { get; init; } = 0.16m;

    [Required, MinLength(1)]
    public List<CreateSaleItemRequest> Items { get; init; } = new();
}

public sealed class CreateSaleItemRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; init; }

    [Range(1, 1_000_000)]
    public int Quantity { get; init; }
}

public sealed record SaleItemResponse(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record SaleResponse(
    int Id,
    int? CustomerId,
    string? CustomerName,
    string PaymentMethod,
    decimal Subtotal,
    decimal Tax,
    decimal Total,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<SaleItemResponse> Items);
