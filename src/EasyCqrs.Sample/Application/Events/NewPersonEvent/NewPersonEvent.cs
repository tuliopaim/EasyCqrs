namespace EasyCqrs.Sample.Application.Events.NewPersonEvent;

public record NewPersonEvent(Guid PersonId) : IEvent;
