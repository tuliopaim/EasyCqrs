using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.UpdatePersonCommand;

public class UpdatePersonCommandInput : ICommandInput<CommandResult>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
}

