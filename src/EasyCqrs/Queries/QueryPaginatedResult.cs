namespace EasyCqrs.Queries;

public class QueryPaginatedResult<TItem> : QueryLisTItem<TItem>
{
    public QueryPagination Pagination { get; set; } = new();
}