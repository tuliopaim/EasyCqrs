using EasyCqrs.Events;

namespace EasyCqrs.Sample.Application.Events.NewPersonEvent;

public class NewPersonEventHandler : IEventHandler<NewPersonEventInput>
{
    private readonly ILogger<NewPersonEventHandler> _logger;

    public NewPersonEventHandler(ILogger<NewPersonEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewPersonEventInput notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Person [{PersonId}] created!", notification.PersonId);

        return Task.CompletedTask;
    }
}