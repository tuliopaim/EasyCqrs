using EasyCqrs.Mediator;

namespace EasyCqrs.Queries;

public interface IQueryHandler<in TQueryInput, TItem> : IMediatorHandler<TQueryInput, TItem>
    where TQueryInput : QueryInput<TItem>
    where TItem : QueryResult, new()
{
}