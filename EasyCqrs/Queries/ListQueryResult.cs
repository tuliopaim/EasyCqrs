namespace EasyCqrs.Queries;

public class ListQueryResult<TResult> : QueryResult
{
    public IEnumerable<TResult> Results { get; set; }
}