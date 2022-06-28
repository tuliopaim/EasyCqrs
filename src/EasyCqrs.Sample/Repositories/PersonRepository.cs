using EasyCqrs.Sample.Domain;

namespace EasyCqrs.Sample.Repositories;

public interface IPersonRepository
{
    IQueryable<Person> GetPeople();
    IEnumerable<Person> GetPeopleByAge(int age);
    void AddPerson(Person person);
}

public class PersonRepository : IPersonRepository
{
    private List<Person> Persons { get; } = new List<Person>();

    public IQueryable<Person> GetPeople()
    {
        return Persons.AsQueryable();
    }

    public IEnumerable<Person> GetPeopleByAge(int age)
    {
        return Persons.Where(x => x.Age == age);
    }
    
    public void AddPerson(Person person)
    {
        Persons.Add(person);
    }
}
