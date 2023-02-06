namespace EasyCqrs.Sample.Application.Events.NewPersonEvent;

public class NewPersonEventHandler : IEventHandler<NewPersonEvent>
{
    private readonly ILogger<NewPersonEventHandler> _logger;

    public NewPersonEventHandler(ILogger<NewPersonEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewPersonEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Person [{PersonId}] created!", notification.PersonId);

        return Task.CompletedTask;
    }
}