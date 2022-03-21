using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandResult : CommandResult
{
    public NewPersonCommandResult(Guid newPersonId)
    {
        NewPersonId = newPersonId;
    }
    
    public Guid NewPersonId { get; set; }
}   