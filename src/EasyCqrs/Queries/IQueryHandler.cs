using EasyCqrs.Mediator;
using MediatR;

namespace EasyCqrs.Queries;

public interface IQueryHandler<in TQueryInput, TQueryResult> : IMediatorHandler<TQueryInput, TQueryResult>
    where TQueryInput : IRequest<TQueryResult>, IMediatorInput<TQueryResult>
    where TQueryResult : IMediatorResult
{
}