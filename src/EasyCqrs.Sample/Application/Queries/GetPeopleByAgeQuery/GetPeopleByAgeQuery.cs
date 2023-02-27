namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public record GetPeopleByAgeQuery(int Age)
    : IQuery<IEnumerable<GetPeopleByAgeQueryItem>>
{
}
