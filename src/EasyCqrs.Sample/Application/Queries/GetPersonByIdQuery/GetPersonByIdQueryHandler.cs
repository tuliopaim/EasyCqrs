using EasyCqrs.Queries;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQueryInput, QueryResult<GetPersonByIdResult>>
{
    private readonly IPersonRepository _personRepository;

    public GetPersonByIdQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    public Task<QueryResult<GetPersonByIdResult>> Handle(GetPersonByIdQueryInput request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPeople().FirstOrDefault(x => x.Id == request.Id);
        
        var personResult = GetPersonByIdResult.FromPerson(person);

        return Task.FromResult(new QueryResult<GetPersonByIdResult>
        {
            Result = personResult
        });
    }
}