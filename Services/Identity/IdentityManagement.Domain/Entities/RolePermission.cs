namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Association between a role and a permission (many-to-many join).
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public DateTimeOffset GrantedAt { get; set; } = DateTimeOffset.UtcNow;
}
