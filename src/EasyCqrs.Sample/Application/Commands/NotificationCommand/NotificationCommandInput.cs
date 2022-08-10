using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Commands.Common;

namespace EasyCqrs.Sample.Application.Commands.NotificationCommand;

public record NotificationCommandInput(string? Notification) : ICommandInput<CommandResult>
{
}
