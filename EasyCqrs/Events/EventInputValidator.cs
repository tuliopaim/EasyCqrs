using EasyCqrs.Mediator;

namespace EasyCqrs.Events;

public class EventInputValidator<TEventInput> : MediatorInputValidator<TEventInput> where TEventInput : EventInput
{
}