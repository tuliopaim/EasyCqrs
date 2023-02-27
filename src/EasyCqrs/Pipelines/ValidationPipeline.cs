using EasyCqrs.Results;
using FluentValidation;
using MediatR;

namespace EasyCqrs.Pipelines;

public class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
           TRequest request,
           RequestHandlerDelegate<TResponse> next,
           CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        Error[] errors = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailures => validationFailures is not null)
            .Select(failure => new Error($"[{failure.PropertyName}] - {failure.ErrorMessage}"))
            .ToArray();

        if (errors.Any())
        {
            var result = CreateValidationResult(errors);

            return result;
        }

        return await next();
    }

    private static TResponse CreateValidationResult(Error[] errors)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (Result.WithErrors(errors) as TResponse)!;
        }

        Type genericType = typeof(TResponse).GenericTypeArguments[0];

        var result = typeof(Result)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name == nameof(Result.WithErrors) &&
                m.IsGenericMethod &&
                m.GetGenericArguments().Length == 1)!
            .MakeGenericMethod(genericType)!
            .Invoke(null, new object?[] { errors })!;

        return (TResponse) result;
    }
}
