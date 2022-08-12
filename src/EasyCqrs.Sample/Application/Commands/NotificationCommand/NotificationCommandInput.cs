using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NotificationCommand;

public record NotificationCommandInput(string? Notification) : ICommandInput<CommandResult>
{
}
