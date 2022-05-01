namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleResult
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}