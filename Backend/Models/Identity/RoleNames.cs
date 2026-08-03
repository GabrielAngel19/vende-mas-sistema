namespace Backend.Models.Identity;

public static class RoleNames
{
    public const string Customer = "Customer";
    public const string StoreOwner = "StoreOwner";
    public const string StoreEmployee = "StoreEmployee";

    public static readonly string[] All =
        [Customer, StoreOwner, StoreEmployee];
}
