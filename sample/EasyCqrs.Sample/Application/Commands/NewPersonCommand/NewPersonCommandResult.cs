using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandResult : CommandResult
{
    public Guid Id { get; set; }
}