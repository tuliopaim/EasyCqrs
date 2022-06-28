using Newtonsoft.Json;

namespace EasyCqrs.Mediator;

public class MediatorResult : IMediatorResult
{
    private readonly List<string> _errors = new();

    public virtual void AddError(string error)
    {
        ArgumentNullException.ThrowIfNull(error);

        _errors.Add(error);
    }

    public void AddError(Exception ex, string error)
    {
        Exception = ex;
        AddError(error);
    }

    [JsonIgnore]
    public Exception? Exception { get; private set; }

    public IReadOnlyList<string> Errors => _errors;
    public bool IsValid => _errors.Count == 0;
}