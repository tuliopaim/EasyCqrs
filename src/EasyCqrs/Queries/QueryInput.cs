using EasyCqrs.Mediator;

namespace EasyCqrs.Queries;

public class QueryInput<TItem> : MediatorInput<TItem> 
    where TItem : QueryResult
{
}