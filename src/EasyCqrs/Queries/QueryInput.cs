using EasyCqrs.Mediator;

namespace EasyCqrs.Queries;

public class QueryInput<TQueryResult> : MediatorInput<TQueryResult> where TQueryResult : QueryResult
{
}