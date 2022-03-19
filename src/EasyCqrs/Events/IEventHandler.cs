using MediatR;

namespace EasyCqrs.Events;

public interface IEventHandler<in TEventInput> : INotificationHandler<TEventInput> where TEventInput : EventInput
{
}