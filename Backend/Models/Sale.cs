namespace Backend.Models;

public sealed class Sale
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "Completada";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
