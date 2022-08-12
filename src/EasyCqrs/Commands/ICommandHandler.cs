using EasyCqrs.Mediator;
using MediatR;

namespace EasyCqrs.Commands;

public interface ICommandHandler<in TCommandInput, TCommandResult> : IRequestHandler<TCommandInput, TCommandResult>
    where TCommandInput : ICommandInput<TCommandResult>, IMediatorInput<TCommandResult>
{
}