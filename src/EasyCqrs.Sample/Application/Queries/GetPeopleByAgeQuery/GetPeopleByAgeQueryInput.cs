using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public class GetPeopleByAgeQueryInput
    : QueryInput<QueryLisTItem<GetPeopleByAgeItem>>
{
    public int Age { get; set; }
}
