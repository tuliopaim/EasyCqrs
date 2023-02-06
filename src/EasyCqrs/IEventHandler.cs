using MediatR;

namespace EasyCqrs;

public interface IEventHandler<in TEventInput> : INotificationHandler<TEventInput> where TEventInput : IEvent
{
}