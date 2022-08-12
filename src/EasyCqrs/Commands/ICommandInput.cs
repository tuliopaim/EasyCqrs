using EasyCqrs.Mediator;

namespace EasyCqrs.Commands;
    
public interface ICommandInput<out TCommandResult> : IMediatorInput<TCommandResult>
{
}