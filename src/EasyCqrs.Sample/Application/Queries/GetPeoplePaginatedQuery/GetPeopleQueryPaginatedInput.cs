using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleQueryPaginatedInput : QueryPaginatedInput<GetPeopleQueryPaginatedResult>
{
    public string? Name { get; set; }
    public int? Age { get; set; }
}