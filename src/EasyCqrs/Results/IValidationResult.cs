namespace EasyCqrs.Results;

public interface IValidationResult
{
    public Error[] Errors { get; }
}
