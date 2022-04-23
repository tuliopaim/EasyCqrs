using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.ExceptionThrownCommand;

public class ExceptionThrownCommandHandler : ICommandHandler<ExceptionThrownCommandInput, CommandResult>
{
    public Task<CommandResult> Handle(
        ExceptionThrownCommandInput request, 
        CancellationToken cancellationToken)
    {
        var divisor = 0;
        _ = 420 / divisor;

        return Task.FromResult(new CommandResult());
    }
}

