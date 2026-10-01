using System.Text;
using IdentityManagement.Application.Common.Interfaces;
using IdentityManagement.Application.Common.Settings;
using IdentityManagement.Domain.Entities;
using IdentityManagement.Infrastructure.Persistence;
using IdentityManagement.Infrastructure.Persistence.Interceptors;
using IdentityManagement.Infrastructure.Repositories;
using IdentityManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityManagement.Infrastructure;

/// <summary>
/// Registers Infrastructure services: EF Core, ASP.NET Core Identity with the password/lockout
/// policy, JWT bearer authentication, repositories, unit of work, and supporting services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Strongly-typed settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

        services.AddHttpContextAccessor();

        // Infrastructure services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        // EF Core
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Identity with password + lockout policy
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password policy
            options.Password.RequiredLength = 12;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 5;

            // Lockout policy
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers = true;

            // User / sign-in
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Repositories & Unit of Work
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application service implementations
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IEmailService, LoggingEmailService>();
        services.AddScoped<ISmsService, LoggingSmsService>();

        // JWT Bearer authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Social / external logins (configure secrets in appsettings / user-secrets).
        AddExternalLogins(services, configuration);

        // Health checks
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database");

        return services;
    }

    private static void AddExternalLogins(IServiceCollection services, IConfiguration configuration)
    {
        var authBuilder = services.AddAuthentication();

        var google = configuration.GetSection("Authentication:Google");
        if (!string.IsNullOrWhiteSpace(google["ClientId"]))
        {
            authBuilder.AddGoogle(o =>
            {
                o.ClientId = google["ClientId"]!;
                o.ClientSecret = google["ClientSecret"]!;
            });
        }

        var microsoft = configuration.GetSection("Authentication:Microsoft");
        if (!string.IsNullOrWhiteSpace(microsoft["ClientId"]))
        {
            authBuilder.AddMicrosoftAccount(o =>
            {
                o.ClientId = microsoft["ClientId"]!;
                o.ClientSecret = microsoft["ClientSecret"]!;
            });
        }

        var facebook = configuration.GetSection("Authentication:Facebook");
        if (!string.IsNullOrWhiteSpace(facebook["AppId"]))
        {
            authBuilder.AddFacebook(o =>
            {
                o.AppId = facebook["AppId"]!;
                o.AppSecret = facebook["AppSecret"]!;
            });
        }

        var twitter = configuration.GetSection("Authentication:Twitter");
        if (!string.IsNullOrWhiteSpace(twitter["ConsumerKey"]))
        {
            authBuilder.AddTwitter(o =>
            {
                o.ConsumerKey = twitter["ConsumerKey"]!;
                o.ConsumerSecret = twitter["ConsumerSecret"]!;
            });
        }
    }
}
