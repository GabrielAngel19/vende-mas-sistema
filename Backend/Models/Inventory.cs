namespace Backend.Models;

public sealed class Inventory
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; } = 5;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Branch Branch { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
