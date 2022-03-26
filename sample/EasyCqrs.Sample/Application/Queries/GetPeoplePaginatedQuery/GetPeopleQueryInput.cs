using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleQueryInput : PagedQueryInput<GetPeopleQueryResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}