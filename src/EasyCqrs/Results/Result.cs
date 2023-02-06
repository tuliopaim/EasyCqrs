namespace EasyCqrs.Results;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result WithError(Error error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue? value) => new(value, true, Error.None);
    public static Result<TValue> WithError<TValue>(Error error) => new(default, false, error);

    public static implicit operator Result(Error error) => WithError(error);
}
