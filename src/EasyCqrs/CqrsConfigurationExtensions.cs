using System.Reflection;
using EasyCqrs.Notifications;
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
            .AddMediator(cqrsConfiguration)
            .AddValidators(cqrsConfiguration)
            .AddPipelines(cqrsConfiguration);
    }
    
    private static IServiceCollection AddMediator(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        return services.AddMediatR(cqrsConfiguration.Assemblies, config =>
        {
            config.AsScoped();
        });
    }

    private static IServiceCollection AddValidators(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        services.AddValidatorsFromAssemblies(cqrsConfiguration.Assemblies);

        return services;
    }

    private static IServiceCollection AddPipelines(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        return services
            .AddScoped<INotifier, Notifier>()
            .AddExceptionPipeline(cqrsConfiguration)
            .AddLogPipelineBehavior(cqrsConfiguration)
            .AddValidationPipeline(cqrsConfiguration)
            .AddNotificationPipeline(cqrsConfiguration);
    }

    private static IServiceCollection AddExceptionPipeline(this IServiceCollection services,
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

    private static IServiceCollection AddNotificationPipeline(this IServiceCollection services,
        CqrsConfiguration cqrsConfiguration)
    {
        if (!cqrsConfiguration.WithNotificationPipeline) return services;

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(NotificationPipelineBehavior<,>));

        return services;
    }
}