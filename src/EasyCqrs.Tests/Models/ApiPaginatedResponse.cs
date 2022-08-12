using EasyCqrs.Queries;

namespace EasyCqrs.Tests.Models;

public class ApiPaginatedResponse<T>
{
    public bool IsSucess { get; set; }
    public List<string> Errors { get; set; } = new();
    public QueryPagination Pagination { get; set; } = new();
    public IEnumerable<T> Result { get; set; } = Enumerable.Empty<T>();
}

