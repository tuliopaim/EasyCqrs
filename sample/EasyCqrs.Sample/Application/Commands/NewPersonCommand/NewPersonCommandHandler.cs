using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Events.NewPersonEvent;
using EasyCqrs.Sample.Domain;
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

        await Task.Delay(1000, cancellationToken);

        var person = new Person(request.Name, request.Age);

        await _mediator.Publish(new NewPersonEventInput { PersonId = person.Id }, cancellationToken);

        return new NewPersonCommandResult { Id = person.Id };
    }
}