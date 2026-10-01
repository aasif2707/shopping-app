namespace IdentityManagement.Domain.Constants;

/// <summary>
/// Well-known default application roles seeded on startup.
/// </summary>
public static class RoleConstants
{
    public const string SuperAdmin = "SuperAdmin";
    public const string StoreAdmin = "StoreAdmin";
    public const string Manager = "Manager";
    public const string User = "User";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SuperAdmin, StoreAdmin, Manager, User
    };
}
