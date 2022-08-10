using Microsoft.Extensions.Logging;

namespace EasyCqrs.Pipelines;

public class PipelineLogService : IPipelineLogService
{
    private readonly ILogger<PipelineLogService> _logger;

    public PipelineLogService(ILogger<PipelineLogService> logger)
    {
        _logger = logger;
    }

    public Task LogBeforeAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{RequestType} - Entering handler!", typeof(TRequest).Name);
        return Task.CompletedTask;
    }

    public Task LogAfterAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{RequestType} - Leaving handler!", typeof(TRequest).Name);
        return Task.CompletedTask;
    }
}