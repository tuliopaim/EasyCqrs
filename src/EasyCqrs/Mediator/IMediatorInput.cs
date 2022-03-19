using MediatR;

namespace EasyCqrs.Mediator;

public interface IMediatorInput<out TMediatorResult> 
    : IRequest<TMediatorResult> where TMediatorResult : IMediatorResult
{

}