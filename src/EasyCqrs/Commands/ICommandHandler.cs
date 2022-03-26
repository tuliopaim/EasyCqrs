using EasyCqrs.Mediator;
using MediatR;

namespace EasyCqrs.Commands;

public interface ICommandHandler<in TCommandInput, TCommandResult> : IMediatorHandler<TCommandInput, TCommandResult>
    where TCommandInput : CommandInput<TCommandResult>, IMediatorInput<TCommandResult>
    where TCommandResult : CommandResult, new()
{
}