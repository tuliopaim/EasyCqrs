namespace EasyCqrs.Pipelines;

public interface IPipelineLogService
{
    Task LogBeforeAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
    Task LogAfterAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
}
