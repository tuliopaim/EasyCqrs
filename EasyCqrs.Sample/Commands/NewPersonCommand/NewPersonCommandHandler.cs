using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Commands.NewPersonCommand;

public class NewPersonCommandHandler : ICommandHandler<NewPersonCommandInput, CommandResult>
{
    private readonly ILogger<NewPersonCommandHandler> _logger;

    public NewPersonCommandHandler(ILogger<NewPersonCommandHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task<CommandResult> Handle(NewPersonCommandInput command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering person...");

        await Task.Delay(1000, cancellationToken);
        
        return new CommandResult();
    }
}