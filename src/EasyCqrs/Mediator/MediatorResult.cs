namespace EasyCqrs.Mediator;

public class MediatorResult : IMediatorResult
{
    private readonly List<string> _errors = new();

    public virtual void AddError(string error)
    {
        ArgumentNullException.ThrowIfNull(error);

        _errors.Add(error);
    }

    public bool IsValid => _errors.Count == 0;

    public IReadOnlyList<string> Errors => _errors;
}