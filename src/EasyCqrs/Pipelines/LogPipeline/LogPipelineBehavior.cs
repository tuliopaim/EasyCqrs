using EasyCqrs.Mediator;
using MediatR;

namespace EasyCqrs.Pipelines;

public class LogPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
{
    private readonly IPipelineLogService _pipelineLogService;

    public LogPipelineBehavior(IPipelineLogService pipelineLogService)
    {
        _pipelineLogService = pipelineLogService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        await _pipelineLogService.LogBeforeAsync(request, cancellationToken);

        var result = await next();

        await _pipelineLogService.LogAfterAsync(request, cancellationToken);

        return result;
    }
}
