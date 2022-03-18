using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Commands.NewPersonCommand;

public class NewPersonCommandInput : CommandInput<CommandResult>
{
    public string Name { get; set; }
    public int Age { get; set; }
}