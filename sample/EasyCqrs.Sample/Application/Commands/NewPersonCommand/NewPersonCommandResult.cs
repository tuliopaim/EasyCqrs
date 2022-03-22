using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandResult : CommandResult
{
    /* To validation and exception works properly */
    public NewPersonCommandResult() {}

    public NewPersonCommandResult(Guid newPersonId)
    {
        NewPersonId = newPersonId;
    }
    
    public Guid NewPersonId { get; set; }
}   