using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Queries.GetPeopleQuery;

public class GetPeopleQueryInput : PagedQueryInput<GetPeopleQueryResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}