namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryItem
{
    public GetPersonByIdQueryItem(Guid id, string name, int age)
    {
        Id = id;
        Name = name;
        Age = age;
    }

    public Guid Id { get; }
    public string Name { get; }
    public int Age { get; }
}   