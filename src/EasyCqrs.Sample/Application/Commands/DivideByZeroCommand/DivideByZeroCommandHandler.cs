using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Commands.Common;

namespace EasyCqrs.Sample.Application.Commands.DivideByZeroCommand;

public class DivideByZeroCommandHandler : ICommandHandler<DivideByZeroCommandInput, CommandResult>
{
    public Task<CommandResult> Handle(
        DivideByZeroCommandInput request,
        CancellationToken cancellationToken)
    {
        var divisor = 0;
        _ = 420 / divisor;

        return Task.FromResult(new CommandResult());
    }
}

