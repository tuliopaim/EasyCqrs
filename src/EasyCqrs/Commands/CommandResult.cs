using EasyCqrs.Mediator;

namespace EasyCqrs.Commands;

public class CommandResult : MediatorResult
{
    public override CommandResult AddError(string error)
    {
        base.AddError(error);
        return this;
    }
}