using EasyCqrs.Commands;
using EasyCqrs.Notifications;
using EasyCqrs.Sample.Application.Commands.Common;

namespace EasyCqrs.Sample.Application.Commands.NotificationCommand;

public class NotificationCommandInputHandler : ICommandHandler<NotificationCommandInput, CommandResult>
{
    private readonly INotifier _notifier;

    public NotificationCommandInputHandler(INotifier notifier)
    {
        _notifier = notifier;
    }

    public Task<CommandResult> Handle(NotificationCommandInput request, CancellationToken cancellationToken)
    {
        _notifier.Notify(request.Notification!);

        return Task.FromResult(new CommandResult());
    }
}