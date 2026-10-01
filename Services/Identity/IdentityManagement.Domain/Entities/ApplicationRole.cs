using Microsoft.AspNetCore.Identity;

namespace IdentityManagement.Domain.Entities;

/// <summary>
/// Application role extending the ASP.NET Core Identity role with a description
/// and dynamic permission associations.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }

    public ApplicationRole(string roleName) : base(roleName) { }

    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
