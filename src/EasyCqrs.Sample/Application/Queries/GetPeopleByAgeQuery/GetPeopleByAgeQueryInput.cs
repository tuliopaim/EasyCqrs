using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public record GetPeopleByAgeQueryInput(int Age)
    : IQueryInput<QueryListResult<GetPeopleByAgeQueryItem>>
{
}
