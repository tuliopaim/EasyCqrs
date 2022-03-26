using EasyCqrs.Mediator;
using EasyCqrs.Notifications;
using MediatR;

namespace EasyCqrs.Pipelines;

public class NotificationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
    where TResponse : IMediatorResult
{
    private readonly INotificator _notificator;

    public NotificationPipelineBehavior(INotificator notificator)
    {
        _notificator = notificator;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var response = await next();

        if (_notificator.IsValid) return response;

        foreach (var result in _notificator.Notifications.Select(x => x.Message))
        {
            response.AddError(result);
        }

        return response;
    }
}