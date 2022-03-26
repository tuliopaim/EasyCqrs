using EasyCqrs.Mediator;

namespace EasyCqrs.Queries;

public class QueryResult<TResult> : QueryResult
{
    public TResult Result { get; set; }
}

public class QueryResult : MediatorResult
{
}