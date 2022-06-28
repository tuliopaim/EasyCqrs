using EasyCqrs.Queries;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public class GetPeopleByAgeyQueryListHandler : IQueryHandler<GetPeopleByAgeQueryInput, QueryListResult<GetPeopleByAgeItem>>
{
    private readonly IPersonRepository _personRepository;

    public GetPeopleByAgeyQueryListHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<QueryListResult<GetPeopleByAgeItem>> Handle(GetPeopleByAgeQueryInput request, CancellationToken cancellationToken)
    {
        var people = _personRepository.GetPeopleByAge(request.Age)
            .Select(x => new GetPeopleByAgeItem(x.Id, x.Name, x.Email, x.Age));

        return Task.FromResult(new QueryListResult<GetPeopleByAgeItem>
        {
            Result = people
        });
    }
}
