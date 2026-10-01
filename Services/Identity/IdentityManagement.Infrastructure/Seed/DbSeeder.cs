using IdentityManagement.Domain.Constants;
using IdentityManagement.Domain.Entities;
using IdentityManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityManagement.Infrastructure.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await context.Database.MigrateAsync();

        foreach (var roleName in RoleConstants.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName)
                {
                    Description = $"{roleName}",
                    IsSystemRole = true
                });
            }
        }
        foreach (var permissionName in PermissionConstants.All)
        {
            if (!await context.Permissions.AnyAsync(p => p.Name == permissionName))
            {
                context.Permissions.Add(new Permission
                {
                    Name = permissionName,
                    Group = permissionName.Split('.')[0],
                    Description = permissionName
                });
            }
        }
        await SeedDefaultUserAsync(
            context,
            userManager,
            email: "superadmin@gmail.com",
            firstName: "Super",
            lastName: "Admin",
            roleName: RoleConstants.SuperAdmin,
            password: "Password@123");
        await SeedDefaultUserAsync(
            context,
            userManager,
            email: "storeadmin@gmail.com",
            firstName: "Store",
            lastName: "Admin",
            roleName: RoleConstants.StoreAdmin,
            password: "Password@123");
        await context.SaveChangesAsync();
    }

    private static async Task SeedDefaultUserAsync(AppDbContext context, UserManager<ApplicationUser> userManager,string email,string firstName,string lastName,string roleName,string password)
    {
        var normalisedEmail = userManager.NormalizeEmail(email);
        var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.NormalizedEmail == normalisedEmail);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true,
                PhoneConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed user '{email}': {errors}");
            }
        }
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            var roleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign role '{roleName}' to '{email}': {errors}");
            }
        }
    }
}