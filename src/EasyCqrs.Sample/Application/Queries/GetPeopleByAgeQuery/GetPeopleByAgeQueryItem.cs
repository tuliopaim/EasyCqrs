namespace EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;

public class GetPeopleByAgeQueryItem
{
    public GetPeopleByAgeQueryItem(Guid id, string name, string email, int age)
    {
        Id = id;
        Name = name;
        Email = email;
        Age = age;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
