namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Direct permission grant to a user (overrides / augments role permissions).
/// </summary>
public class UserPermission
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    /// <summary>When false, explicitly denies a permission the user would otherwise inherit.</summary>
    public bool IsGranted { get; set; } = true;

    public DateTimeOffset GrantedAt { get; set; } = DateTimeOffset.UtcNow;
}
