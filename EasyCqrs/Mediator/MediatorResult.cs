namespace EasyCqrs.Mediator;

public class MediatorResult : IMediatorResult
{
    private readonly List<string> _errors = new();

    public virtual IMediatorResult AddError(string error)
    {
        _errors.Add(error);
        return this;
    }

    public bool IsValid()
    {
        return !Errors.Any();
    }

    public IEnumerable<string> Errors => _errors;
}