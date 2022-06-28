namespace EasyCqrs.Queries;

public abstract class QueryPaginatedInput<TItem> : QueryInput<TItem>
    where TItem : QueryResult
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}