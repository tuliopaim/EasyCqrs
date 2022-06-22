using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public class GetPeopleByAgeQueryInput
    : QueryInput<QueryListResult<GetPeopleByAgeItem>>
{
    public int Age { get; set; }
}
