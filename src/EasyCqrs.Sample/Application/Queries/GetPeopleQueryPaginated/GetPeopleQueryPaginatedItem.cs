namespace EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;

public class GetPeopleQueryPaginatedItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}