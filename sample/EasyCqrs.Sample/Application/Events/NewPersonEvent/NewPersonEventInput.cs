using EasyCqrs.Events;
using FluentValidation;

namespace EasyCqrs.Sample.Application.Events.NewPersonEvent;

public class NewPersonEventInput : EventInput
{
    public Guid PersonId { get; set; }
}