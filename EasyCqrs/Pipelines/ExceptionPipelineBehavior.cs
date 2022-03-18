using EasyCqrs.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyCqrs.Pipelines;

public class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
    where TResponse : MediatorResult, new()
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
            return HandleException(ex);
        }
    }

    private TResponse HandleException(Exception ex) 
    {
        _logger.LogError(ex, "{RequestType} - Exception captured!", typeof(TRequest).Name);

        var result = new TResponse();

        result.AddError("Ocorreu um erro!");

        return result;
    }
}