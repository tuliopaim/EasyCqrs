using EasyCqrs.Commands;
using EasyCqrs.Notifications;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Commands.UpdatePersonCommand;

public class UpdatePersonCommandHandler : ICommandHandler<UpdatePersonCommandInput, CommandResult>
{
    private readonly IPersonRepository _personRepository;
    private readonly INotifier _notifier;

    public UpdatePersonCommandHandler(
        IPersonRepository personRepository,
        INotifier notifier)
    {
        _personRepository = personRepository;
        _notifier = notifier;
    }

    public async Task<CommandResult> Handle(UpdatePersonCommandInput request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPersonById(request.Id);

        if (person is null)
        {
            _notifier.Notify("Person not found!");
            return new();
        }

        person.Update(request.Name!, request.Email!, request.Age);

        _personRepository.UpdatePerson(person);

        return new();
    }
}

