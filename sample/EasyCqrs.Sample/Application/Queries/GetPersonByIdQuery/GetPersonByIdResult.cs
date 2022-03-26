using EasyCqrs.Sample.Domain;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdResult
{
    public GetPersonByIdResult(Guid id, string name, int age)
    {
        Id = id;
        Name = name;
        Age = age;
    }

    public Guid Id { get; }
    public string Name { get; }
    public int Age { get; }

    public static GetPersonByIdResult? FromPerson(Person? person)
    {
        return person is null 
            ? null
            : new GetPersonByIdResult(person.Id, person.Name, person.Age);
    }
}   