using System.Reflection;

namespace EasyCqrs;

public class CqrsConfiguration
{   
    public CqrsConfiguration(Assembly[] assemblies)
    {
        if (assemblies is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        Assemblies = assemblies;
    }
    
    internal Assembly[] Assemblies { get; }
    internal bool WithLogPipeline { get; private set; } = true;
    internal bool WithValidationPipeline { get; private set; } = true;
    internal bool WithNotificationPipeline { get; private set; } = true;
    internal bool WithExceptionPipeline { get; private set; } = true;

    public void DisableLogPipeline()
    {
        WithLogPipeline = false;
    }
    
    public void DisableValidationPipeline()
    {
        WithValidationPipeline = false;
    }

    public void DisableNotificationPipeline()
    {
        WithNotificationPipeline = false;
    }

    public void DisableExceptionPipeline()
    {
        WithExceptionPipeline = false;
    }
}