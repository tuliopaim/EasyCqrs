namespace EasyCqrs.Sample.Domain;

public class Person
{
    public Person(string name, int age)
    {
        Id = Guid.NewGuid();
        Name = name;
        Age = age;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public int Age { get; private set; }
}