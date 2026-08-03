using Backend.Models.Identity;

namespace Backend.Models;

public sealed class StoreUser
{
    public int StoreId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = RoleNames.StoreEmployee;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Store Store { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
