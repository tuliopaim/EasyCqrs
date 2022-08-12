namespace EasyCqrs.Queries;

public abstract class QueryPaginatedInput<TItem> : IQueryInput<TItem>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}