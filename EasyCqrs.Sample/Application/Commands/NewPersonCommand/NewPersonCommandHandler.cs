using EasyCqrs.Commands;
using EasyCqrs.Sample.Events;
using MediatR;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandHandler : ICommandHandler<NewPersonCommandInput, CommandResult>
{
    private readonly ILogger<NewPersonCommandHandler> _logger;
    private readonly IMediator _mediator;

    public NewPersonCommandHandler(ILogger<NewPersonCommandHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<CommandResult> Handle(NewPersonCommandInput request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering person...");

        await Task.Delay(1000, cancellationToken);

        await _mediator.Publish(new NewPersonEventInput { PersonId = Guid.Empty }, cancellationToken);

        return new CommandResult();
    }
}