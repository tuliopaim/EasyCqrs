namespace EasyCqrs.Results;

public class Result
{
    protected Result(bool isSuccess, params Error[] errors)
    {
        if (isSuccess && errors.Any() ||
            !isSuccess && !errors.Any())
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;

        _errors.AddRange(errors);
    }

    public bool IsSuccess { get; }

    private readonly List<Error> _errors = new();
    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    public static Result Success() => new(true);
    public static Result WithError(Error error) => new(false, error);
    public static Result WithErrors(Error[] errors) => new(false, errors);
    public static Result<TValue> Success<TValue>(TValue? value) => new(value, true);
    public static Result<TValue> WithError<TValue>(Error error) => new(default, false, error);
    public static Result<TValue> WithErrors<TValue>(Error[] errors) => new(default, false, errors);

    public static implicit operator Result(Error error) => WithError(error);
    public static implicit operator Result(Error[] errors) => WithErrors(errors);
}
