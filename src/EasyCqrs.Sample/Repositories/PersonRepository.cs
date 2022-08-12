using EasyCqrs.Sample.Domain;

namespace EasyCqrs.Sample.Repositories;

public interface IPersonRepository
{
    IQueryable<Person> GetPeople();
    IEnumerable<Person> GetPeopleByAge(int age);
    void AddPerson(Person person);
    void UpdatePerson(Person person);
    Person? GetPersonById(Guid id);
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
 
    public Person? GetPersonById(Guid id)
    {
        return Persons.FirstOrDefault(x => x.Id == id);
    }
   
    public void AddPerson(Person person)
    {
        Persons.Add(person);
    }

    public void UpdatePerson(Person person)
    {
        Persons.Remove(Persons.Find(p => p.Id == person.Id)!);
        Persons.Add(person);
    }
}
