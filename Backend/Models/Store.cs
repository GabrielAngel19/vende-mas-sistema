using Backend.Models.Identity;

namespace Backend.Models;

public sealed class Store
{
    public int Id { get; set; }
    public int? OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ApplicationUser? OwnerUser { get; set; }
    public ICollection<StoreUser> Users { get; set; } = new List<StoreUser>();
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
