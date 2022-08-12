using EasyCqrs.Commands;
using EasyCqrs.Notifications;
using EasyCqrs.Sample.Application.Commands.Common;
using EasyCqrs.Sample.Application.Events.NewPersonEvent;
using EasyCqrs.Sample.Domain;
using EasyCqrs.Sample.Repositories;
using MediatR;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandHandler : ICommandHandler<NewPersonCommandInput, CreatedCommandResult>
{
    private readonly INotifier _notifier;
    private readonly IPersonRepository _personRepository;
    private readonly IMediator _mediator;

    public NewPersonCommandHandler(
        INotifier notifier,
        IPersonRepository personRepository,
        IMediator mediator)
    {
        _notifier = notifier;
        _personRepository = personRepository;
        _mediator = mediator;
    }

    public async Task<CreatedCommandResult> Handle(NewPersonCommandInput request, CancellationToken cancellationToken)
    {
        if (ExistsOtherPersonWithSameEmail(request))
        {
            _notifier.Notify("Person with the same email already added!");

            return new CreatedCommandResult();
        }

        var person = new Person(request.Name!, request.Email!, request.Age);

        _personRepository.AddPerson(person);

        await _mediator.Publish(new NewPersonEventInput { PersonId = person.Id }, cancellationToken);

        return new CreatedCommandResult { Id = person.Id };
    }

    private bool ExistsOtherPersonWithSameEmail(NewPersonCommandInput request)
    {
        return _personRepository.GetPeople().Any(x => x.Email == request.Email);
    }
}