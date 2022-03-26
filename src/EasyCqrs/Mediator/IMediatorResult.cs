namespace EasyCqrs.Mediator;

public interface IMediatorResult
{
    bool IsValid { get; }
    IReadOnlyList<string> Errors { get; }
    void AddError(string error);

}