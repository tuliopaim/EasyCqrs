namespace EasyCqrs.Queries;

public class QueryResult<TItem>
{
    public TItem? Result { get; set; }

    public static implicit operator QueryResult<TItem>(TItem? result)
    {
        return new QueryResult<TItem> { Result = result };
    }
}
