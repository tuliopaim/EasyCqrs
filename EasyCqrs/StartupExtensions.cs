using System.Reflection;
using EasyCqrs.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using IMediator = EasyCqrs.Mediator.IMediator;

namespace EasyCqrs;

public static class StartupExtensions
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

        var configuration = new CqrsConfiguration(assemblies);

        config?.Invoke(configuration);

        services.AddScoped<IMediator, Mediator.Mediator>();
        services.AddMediatR(configuration.Assemblies);

        return services
            .AddExceptionPipelineBehavior(configuration)
            .AddLogPipelineBehavior(configuration)
            .AddValidationPipeline(configuration)
            .AddValidators(configuration);
    }

    private static IServiceCollection AddExceptionPipelineBehavior(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        if (!cqrsConfiguration.WithExceptionPipeline) return services;

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionPipelineBehavior<,>));

        return services;
    }

    private static IServiceCollection AddLogPipelineBehavior(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        if (!cqrsConfiguration.WithLogPipeline) return services;

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogPipelineBehavior<,>));

        return services;
    }

    private static IServiceCollection AddValidationPipeline(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        if (!cqrsConfiguration.WithValidationPipeline) return services;

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        foreach (var assemblyScanResult in AssemblyScanner.FindValidatorsInAssemblies(cqrsConfiguration.Assemblies))
        {
            services.AddScoped(assemblyScanResult.InterfaceType, assemblyScanResult.ValidatorType);
        }

        return services;
    }
}