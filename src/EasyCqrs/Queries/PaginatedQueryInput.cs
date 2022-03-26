namespace EasyCqrs.Queries;

public abstract class PaginatedQueryInput<TQueryResult> : QueryInput<TQueryResult>
    where TQueryResult : QueryResult
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}