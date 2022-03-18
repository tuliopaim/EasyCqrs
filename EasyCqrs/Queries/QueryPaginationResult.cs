using Newtonsoft.Json;

namespace EasyCqrs.Queries;

public class QueryPaginationResult
{
    public int TotalElements { get; set; }
    public int Size { get; set; }
    public int Number { get; set; }
    
    public int TotalPages => Size == 0
        ? 0
        : (int)Math.Ceiling(TotalElements / (double)Size);

    [JsonIgnore]
    public int FirstPage => 0;

    [JsonIgnore]
    public int LastPage => TotalPages == 0 ? 0 : TotalPages - 1;

    [JsonIgnore]
    public bool HasPrevPage => Number >= 1;

    [JsonIgnore]
    public bool HasNextPage => Number < LastPage;

    [JsonIgnore]
    public int PrevPage => !HasPrevPage ? FirstPage : Number - 1;

    [JsonIgnore]
    public int NextPage => !HasNextPage ? LastPage : Number + 1;
}