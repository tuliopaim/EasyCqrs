namespace EasyCqrs.Tests.Models;

public class ApiListResponse<T>
{
    public bool IsSucess { get; set; }
    public List<string> Errors { get; set; } = new();
    public IEnumerable<T> Result { get; set; } = Enumerable.Empty<T>();
}

