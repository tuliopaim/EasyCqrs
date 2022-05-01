namespace EasyCqrs.Sample.Domain;

public class Person
{
    public Person(string name, string email, int age)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Age = age;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public int Age { get; private set; }
}