using EasyCqrs.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EasyCqrs.Pipelines;

public class LogPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
    where TResponse : IMediatorResult
{
    private readonly ILogger<LogPipelineBehavior<TRequest, TResponse>> _logger;

    public LogPipelineBehavior(ILogger<LogPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogDebug("{RequestType} - Entering handler... {@Request}", typeof(TRequest).Name, request);

        var result = await next();

        _logger.LogDebug("{RequestType} - Leaving handler!", typeof(TRequest).Name);

        return result;
    }
}