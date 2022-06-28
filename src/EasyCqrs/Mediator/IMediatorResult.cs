namespace EasyCqrs.Mediator;

public interface IMediatorResult
{
    Exception? Exception { get; }
    bool IsValid { get; }
    IReadOnlyList<string> Errors { get; }
    void AddError(string error);
    void AddError(Exception ex, string error = "An exception has occurred!");
}