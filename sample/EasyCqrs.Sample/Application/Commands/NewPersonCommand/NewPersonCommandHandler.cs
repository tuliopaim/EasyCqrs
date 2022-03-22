using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Events;
using MediatR;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandHandler : ICommandHandler<NewPersonCommandInput, NewPersonCommandResult>
{
    private readonly ILogger<NewPersonCommandHandler> _logger;
    private readonly IMediator _mediator;

    public NewPersonCommandHandler(ILogger<NewPersonCommandHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<NewPersonCommandResult> Handle(NewPersonCommandInput request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering person...");

        var personId = Guid.NewGuid();

        await Task.Delay(1000, cancellationToken);
        
        return new NewPersonCommandResult(personId);
    }
}