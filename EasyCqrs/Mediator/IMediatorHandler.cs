using MediatR;

namespace EasyCqrs.Mediator;

public interface IMediatorHandler<in TMediatorInput, TMediatorResult> : IRequestHandler<TMediatorInput, TMediatorResult>
    where TMediatorInput : IRequest<TMediatorResult>, IMediatorInput<TMediatorResult>
    where TMediatorResult : IMediatorResult
{
}   