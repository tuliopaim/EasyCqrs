using MediatR;

namespace EasyCqrs.Mediator;

public class Mediator : MediatR.Mediator, IMediator
{
    public Mediator(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }
}