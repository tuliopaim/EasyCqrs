namespace EasyCqrs.Queries;

public abstract class QueryPaginatedInput<TQueryResult> : QueryInput<TQueryResult>
    where TQueryResult : QueryResult
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}