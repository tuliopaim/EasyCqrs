using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandInput : CommandInput<NewPersonCommandResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}