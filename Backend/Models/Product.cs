namespace Backend.Models;

public sealed class Product
{
    public int Id { get; set; }
    public int? StoreId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Unit { get; set; } = "pieza";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public Store? Store { get; set; }
    public Category? CategoryEntity { get; set; }
    public ICollection<ProductAttribute> Attributes { get; set; } =
        new List<ProductAttribute>();
    public ICollection<Inventory> Inventories { get; set; } =
        new List<Inventory>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } =
        new List<InventoryMovement>();
}
