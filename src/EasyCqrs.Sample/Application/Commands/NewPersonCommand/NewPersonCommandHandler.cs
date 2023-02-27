using EasyCqrs.Results;
using EasyCqrs.Sample.Application.Events.NewPersonEvent;
using EasyCqrs.Sample.Domain;
using EasyCqrs.Sample.Repositories;
using MediatR;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandHandler : ICommandHandler<NewPersonCommand, Guid>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMediator _mediator;

    public NewPersonCommandHandler(
        IPersonRepository personRepository,
        IMediator mediator)
    {
        _personRepository = personRepository;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(NewPersonCommand request, CancellationToken cancellationToken)
    {
        if (ExistsOtherPersonWithSameEmail(request))
        {
            return new Error("Person with the same email already added!");
        }

        var person = new Person(request.Name!, request.Email!, request.Age);

        _personRepository.AddPerson(person);

        await _mediator.Publish(new NewPersonEvent(person.Id), cancellationToken);

        return Result.Success(person.Id);
    }

    private bool ExistsOtherPersonWithSameEmail(NewPersonCommand request)
    {
        return _personRepository.GetPeople().Any(x => x.Email == request.Email);
    }
}
