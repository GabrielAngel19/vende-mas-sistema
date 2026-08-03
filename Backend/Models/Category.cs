namespace Backend.Models;

public sealed class Category
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Store Store { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
