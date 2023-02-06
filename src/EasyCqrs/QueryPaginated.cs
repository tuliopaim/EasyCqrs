using InteligenteZap.Domain.Shared;

namespace EasyCqrs;

public class QueryPaginated<TResponse> : IQuery<PaginatedList<TResponse>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
