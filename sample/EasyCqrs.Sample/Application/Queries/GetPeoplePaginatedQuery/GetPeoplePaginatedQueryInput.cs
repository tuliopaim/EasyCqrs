using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeoplePaginatedQueryInput : PaginatedQueryInput<GetPeoplePaginatedQueryResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}