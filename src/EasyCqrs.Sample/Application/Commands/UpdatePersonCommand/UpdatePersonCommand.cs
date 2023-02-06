namespace EasyCqrs.Sample.Application.Commands.UpdatePersonCommand;

public class UpdatePersonCommand : ICommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
}

