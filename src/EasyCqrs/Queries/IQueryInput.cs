using EasyCqrs.Mediator;

namespace EasyCqrs.Queries;

public interface IQueryInput<out TItem> : IMediatorInput<TItem>
{
}