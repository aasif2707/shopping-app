namespace IdentityManagement.Domain.Constants;

/// <summary>
/// Canonical permission names used for policy-based authorization.
/// Format: "{Resource}.{Action}".
/// </summary>
public static class PermissionConstants
{
    public const string UsersView = "Users.View";
    public const string UsersManage = "Users.Manage";
    public const string RolesView = "Roles.View";
    public const string RolesManage = "Roles.Manage";
    public const string PermissionsManage = "Permissions.Manage";
    public const string AuditView = "Audit.View";

    public static readonly IReadOnlyList<string> All = new[]
    {
        UsersView, UsersManage, RolesView, RolesManage, PermissionsManage, AuditView
    };
}
