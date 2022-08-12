using EasyCqrs.Queries;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public class GetPeopleByAgeyQueryListHandler : IQueryHandler<GetPeopleByAgeQueryInput, QueryListResult<GetPeopleByAgeQueryItem>>
{
    private readonly IPersonRepository _personRepository;

    public GetPeopleByAgeyQueryListHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<QueryListResult<GetPeopleByAgeQueryItem>> Handle(GetPeopleByAgeQueryInput request, CancellationToken cancellationToken)
    {
        var people = _personRepository.GetPeopleByAge(request.Age)
            .Select(x => new GetPeopleByAgeQueryItem(x.Id, x.Name, x.Email, x.Age));

        return Task.FromResult(new QueryListResult<GetPeopleByAgeQueryItem>
        {
            Result = people
        });
    }
}
