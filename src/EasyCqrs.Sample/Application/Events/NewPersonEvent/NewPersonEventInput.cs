using EasyCqrs.Events;

namespace EasyCqrs.Sample.Application.Events.NewPersonEvent;

public class NewPersonEventInput : IEventInput
{
    public Guid PersonId { get; set; }
}