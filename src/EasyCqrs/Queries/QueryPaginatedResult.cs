namespace EasyCqrs.Queries;

public class QueryPaginatedResult<TResult> : QueryListResult<TResult>
{
    public QueryPagination Pagination { get; set; } = new();
}