using Newtonsoft.Json;

namespace EasyCqrs.Queries;

public class QueryPagination
{
    public int TotalElements { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    
    public int TotalPages => PageSize == 0
        ? 0
        : (int)Math.Ceiling(TotalElements / (double)PageSize);

    [JsonIgnore]
    public int FirstPage => 0;

    [JsonIgnore]
    public int LastPage => TotalPages == 0 ? 0 : TotalPages - 1;

    [JsonIgnore]
    public bool HasPrevPage => PageNumber >= 1;

    [JsonIgnore]
    public bool HasNextPage => PageNumber < LastPage;

    [JsonIgnore]
    public int PrevPage => !HasPrevPage ? FirstPage : PageNumber - 1;

    [JsonIgnore]
    public int NextPage => !HasNextPage ? LastPage : PageNumber + 1;
}