using EasyCqrs.Mediator;
using EasyCqrs.Notifications;
using MediatR;

namespace EasyCqrs.Pipelines;

public class NotificationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
    where TResponse : IMediatorResult
{
    private readonly INotifier _notifier;

    public NotificationPipelineBehavior(INotifier notifier)
    {
        _notifier = notifier;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var response = await next();

        if (_notifier.IsValid) return response;

        foreach (var notification in _notifier.Notifications)
        {
            if (response.Errors.Contains(notification.Message)) continue;
            response.AddError(notification.Message);
        }

        return response;
    }
}