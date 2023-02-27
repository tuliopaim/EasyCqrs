using EasyCqrs.Results;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public class GetPeopleByAgeyQueryListHandler : IQueryHandler<GetPeopleByAgeQuery, IEnumerable<GetPeopleByAgeQueryItem>>
{
    private readonly IPersonRepository _personRepository;

    public GetPeopleByAgeyQueryListHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<Result<IEnumerable<GetPeopleByAgeQueryItem>>> Handle(GetPeopleByAgeQuery request, CancellationToken cancellationToken)
    {
        var people = _personRepository.GetPeopleByAge(request.Age)
            .Select(x => new GetPeopleByAgeQueryItem(x.Id, x.Name, x.Email, x.Age));

        return Task.FromResult(Result.Success(people));
    }
}
