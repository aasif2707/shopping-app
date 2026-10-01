namespace IdentityManagement.Application.Features.Administration.Dtos;

public record UserSearchRequestDto(
    string? Search,
    bool? IsActive,
    bool? EmailConfirmed,
    int PageNumber = 1,
    int PageSize = 20);

public record UserListItemDto(
    Guid Id,
    string UserName,
    string Email,
    string FullName,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    bool PhoneConfirmed,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);

public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole);

public record CreateRoleRequestDto(
    string Name,
    string? Description);

public record UpdateRoleRequestDto(
    string Name,
    string? Description);

public record SetUserRoleRequestDto(
    Guid UserId,
    string RoleName);
