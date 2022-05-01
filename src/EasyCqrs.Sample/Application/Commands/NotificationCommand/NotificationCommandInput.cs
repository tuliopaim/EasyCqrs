using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NotificationCommand;

public class NotificationCommandInput : CommandInput<CommandResult>
{
    public string? Notification { get; set; }

    public NotificationCommandInput(string? notification)
    {
        Notification = notification;
    }
}
