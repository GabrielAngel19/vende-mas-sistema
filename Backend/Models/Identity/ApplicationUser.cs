using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models.Identity;

public sealed class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<Store> OwnedStores { get; set; } = new List<Store>();
    public ICollection<StoreUser> StoreMemberships { get; set; } =
        new List<StoreUser>();
}
