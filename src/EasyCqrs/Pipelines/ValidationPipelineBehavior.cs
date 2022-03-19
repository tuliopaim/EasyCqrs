using EasyCqrs.Mediator;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyCqrs.Pipelines;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
    where TResponse : MediatorResult, new()
{
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>>? _validators;

    public ValidationPipelineBehavior(
        ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger,
        IEnumerable<IValidator<TRequest>>? validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,    
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validators is null) return next();
        
        var errors = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .ToList();

        _logger.LogDebug("{RequestType} - Validated with {ValidationErrorQuantity} error(s)",
            typeof(TRequest).Name,
            errors.Count);

        return errors.Any() ? ReturnError(errors) : next();
    }

    private Task<TResponse> ReturnError(IEnumerable<ValidationFailure> failures)
    {
        var result = new TResponse();

        foreach (var fail in failures)
        {
            _logger.LogError("{RequestType} - Validation error: {ValidationError}",
                typeof(TRequest).Name,
                fail.ErrorMessage);

            result.AddError(fail.ErrorMessage);
        }

        return Task.FromResult(result);
    }
}