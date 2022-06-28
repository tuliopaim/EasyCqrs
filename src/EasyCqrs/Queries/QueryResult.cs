using EasyCqrs.Mediator;

namespace EasyCqrs.Queries;

public class QueryResult<TItem> : QueryResult
{
    public TItem? Result { get; set; }

    public static implicit operator QueryResult<TItem>(TItem? result)
    {
        return new() { Result = result };
    }
}

public abstract class QueryResult : MediatorResult
{
}

