namespace Backend.Models;

public sealed class Branch
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Store Store { get; set; } = null!;
    public ICollection<Inventory> Inventories { get; set; } =
        new List<Inventory>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
