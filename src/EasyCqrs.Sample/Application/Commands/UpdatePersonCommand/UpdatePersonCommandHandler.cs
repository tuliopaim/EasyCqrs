using EasyCqrs.Results;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Commands.UpdatePersonCommand;

public class UpdatePersonCommandHandler : ICommandHandler<UpdatePersonCommand>
{
    private readonly IPersonRepository _personRepository;

    public UpdatePersonCommandHandler(
        IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPersonById(request.Id);

        if (person is null)
        {
            return new Error("Person not found!");
        }

        person.Update(request.Name!, request.Email!, request.Age);

        _personRepository.UpdatePerson(person);

        return Result.Success();
    }
}

