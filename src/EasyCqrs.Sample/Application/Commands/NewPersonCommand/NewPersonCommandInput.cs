namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public record NewPersonCommandInput(string? Name, string? Email, int Age) : ICommand<Guid>
{
}
