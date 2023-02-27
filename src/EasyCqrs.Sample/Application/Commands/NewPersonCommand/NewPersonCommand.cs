namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public record NewPersonCommand(string? Name, string? Email, int Age) : ICommand<Guid>;

