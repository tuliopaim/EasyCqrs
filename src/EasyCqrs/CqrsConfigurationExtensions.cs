using System.Reflection;
using EasyCqrs.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EasyCqrs;

public static class CqrsConfigurationExtensions
{
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly assembly,
        Action<CqrsConfiguration>? config = null)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return services.AddCqrs(new[] { assembly }, config);
    }

    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<CqrsConfiguration>? config = null)
    {
        if (assemblies is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        var cqrsConfiguration = new CqrsConfiguration(assemblies);

        config?.Invoke(cqrsConfiguration);

        return services
            .AddValidatorsFromAssemblies(cqrsConfiguration.Assemblies)
            .AddValidationPipeline(cqrsConfiguration)
            .AddMediatR(cqrsConfiguration.Assemblies, config =>config.AsScoped());
    }

    private static IServiceCollection AddValidationPipeline(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        if (!cqrsConfiguration.WithValidationPipeline) return services;

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

        return services;
    }
}