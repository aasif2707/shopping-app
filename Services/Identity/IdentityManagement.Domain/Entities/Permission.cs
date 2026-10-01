using IdentityManagement.Domain.Common;

namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Dynamic permission definition used for policy-based authorization.
/// </summary>
public class Permission : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Group { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
