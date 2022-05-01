using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandInput : CommandInput<NewPersonCommandResult>
{
    public NewPersonCommandInput(string? name, string? email, int age)
    {
        Name = name;
        Email = email;
        Age = age;
    }

    public string? Name { get; }
    public string? Email { get; }
    public int Age { get; }
}