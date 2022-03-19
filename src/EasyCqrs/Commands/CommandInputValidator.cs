using EasyCqrs.Mediator;

namespace EasyCqrs.Commands;

public class CommandInputValidator<TCommandInput> : MediatorInputValidator<TCommandInput>
{
}