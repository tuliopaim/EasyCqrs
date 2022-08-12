using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Commands.Common;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public record NewPersonCommandInput(string? Name, string? Email, int Age) :
    ICommandInput<CreatedCommandResult>
{
}