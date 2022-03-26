using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandInput : CommandInput<NewPersonCommandResult>
{
    public NewPersonCommandInput(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name { get; set; }
    public int Age { get; set; }
}