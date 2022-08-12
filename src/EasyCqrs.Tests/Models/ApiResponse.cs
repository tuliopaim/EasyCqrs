namespace EasyCqrs.Tests.Models;

public class ApiResponse<T>
{
    public bool IsSucess { get; set; }
    public List<string> Errors { get; set; } = new();
    public T? Result { get; set; }
}

