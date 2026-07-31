namespace Backend.Models;

public sealed class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? SaleId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public Sale? Sale { get; set; }
}
