using System.Reflection;
using FluentValidation;
using IdentityManagement.Application.Common.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityManagement.Application;

/// <summary>
/// Registers Application-layer services: MediatR handlers, FluentValidation validators,
/// AutoMapper profiles, and the validation pipeline behaviour.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        return services;
    }
}
