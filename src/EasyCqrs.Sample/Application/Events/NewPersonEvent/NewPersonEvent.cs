namespace EasyCqrs.Sample.Application.Events.NewPersonEvent;

public class NewPersonEvent : IEvent
{
    public Guid PersonId { get; set; }
}