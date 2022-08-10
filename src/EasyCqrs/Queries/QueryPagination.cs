namespace EasyCqrs.Queries;

public class QueryPagination
{
    public int TotalElements { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    
    public int TotalPages => PageSize == 0
        ? 0
        : (int)Math.Ceiling(TotalElements / (double)PageSize);

    public int FirstPage => 0;

    public int LastPage => TotalPages == 0 ? 0 : TotalPages - 1;

    public bool HasPrevPage => PageNumber >= 1;

    public bool HasNextPage => PageNumber < LastPage;

    public int PrevPage => !HasPrevPage ? FirstPage : PageNumber - 1;

    public int NextPage => !HasNextPage ? LastPage : PageNumber + 1;
}