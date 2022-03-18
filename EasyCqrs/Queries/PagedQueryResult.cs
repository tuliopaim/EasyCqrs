namespace EasyCqrs.Queries;

public class PagedQueryResult<TResult> : ListQueryResult<TResult>
{
    public QueryPaginationResult Pagination { get; set; }
}