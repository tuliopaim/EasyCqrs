using EasyCqrs.Mediator;

namespace EasyCqrs.Commands;

public class CommandInput<TCommandResult> : MediatorInput<TCommandResult> where TCommandResult : CommandResult
{
}