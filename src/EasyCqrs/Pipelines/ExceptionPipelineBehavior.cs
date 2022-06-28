using EasyCqrs.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EasyCqrs.Pipelines;

public class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
    where TResponse : IMediatorResult, new()
{
    private readonly ILogger<ExceptionPipelineBehavior<TRequest, TResponse>> _logger;

    public ExceptionPipelineBehavior(ILogger<ExceptionPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            return TratarException(ex, request);
        }
    }

    private TResponse TratarException(Exception ex, TRequest request)
    {
        request.MaskSensitiveStrings();

        _logger.LogError(ex, "{RequestType} - Exception captured! {@RequestInput}", typeof(TRequest).Name, request);

        var result = new TResponse();

        result.AddError(ex);

        return result;
    }
}