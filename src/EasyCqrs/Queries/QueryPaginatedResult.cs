namespace EasyCqrs.Queries;

public class QueryPaginatedResult<TItem> : QueryListResult<TItem>
{
    public QueryPagination Pagination { get; set; } = new();
}