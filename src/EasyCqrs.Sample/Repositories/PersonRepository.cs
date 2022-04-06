using EasyCqrs.Sample.Domain;

namespace EasyCqrs.Sample.Repositories;

public interface IPersonRepository
{
    IQueryable<Person> GetPeople();
    void AddPerson(Person person);
}

public class PersonRepository : IPersonRepository
{
    private List<Person> Persons { get; } = new List<Person>();

    public IQueryable<Person> GetPeople()
    {
        return Persons.AsQueryable();
    }

    public void AddPerson(Person person)
    {
        Persons.Add(person);
    }
}
