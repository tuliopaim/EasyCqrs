namespace EasyCqrs.Queries;

public class PaginatedQueryResult<TResult> : ListQueryResult<TResult>
{
    public QueryPagination Pagination { get; set; } = new();
}