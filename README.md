# EasyCqrs
A library to work easier with CQRS on top of MediatR.

## Install

### CLI

``` 
dotnet add package TP.EasyCqrs
``` 

### Package Manager Console

```
Install-Package TP.EasyCqrs
``` 

---
## Usage

### Registering

You can use one of the AddCqrs extension methods on IServiceCollection to inject the required services in the DI container.

You need to pass the Assemblies where the CQRS classes are located.

``` csharp
builder.Services.AddCqrs(typeof(NewPersonCommandHandler).Assembly);
``` 

You can easly disable some of the pipeline behaviores with a configuration lambda:

```csharp
builder.Services.AddCqrs(
    typeof(NewPersonCommandHandler).Assembly, 
    config => {
        config.DisableLogPipeline();
        config.DisableValidationPipeline();
        config.DisableExceptionPipeline();
    });
```

---
## Commands

### CommandResult
You can create a specifc CommandResult class by inheriting the `CommandResult` class.
   
> :warning: **To validation and exception pipelines work properly you need to declare a parameterless contructor**, otherwise the code will not compile! :warning:

```csharp
using EasyCqrs.Commands;

namespace EasyCqrs.Sample.Application.Commands.NewPersonCommand;

public class NewPersonCommandResult : CommandResult
{
    /* To validation and exception works properly */
    public NewPersonCommandResult() {}

    public NewPersonCommandResult(Guid newPersonId)
    {
        NewPersonId = newPersonId;
    }
    
    public Guid NewPersonId { get; set; }
}   
```

    Specifc CommandResult class are not required, you can use the CommandResult instead.

### CommandInput
You create a Command Input class by inheriting from `CommandInput<TResult>` class and specifying the result type, in this example `NewPersonCommandResult`

```csharp
public class NewPersonCommandInput : CommandInput<NewPersonCommandResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}
```

### CommandInputValidator
You can also create a Input Validator using FluentValidation sintax. It will be used in the Validator Pipeline to validate the Input.

``` csharp
public class NewPersonCommandInputValidator : CommandInputValidator<NewPersonCommandInput>
{
    public NewPersonCommandInputValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(150);

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18);
    }
}
``` 

    The InputValidatior class is also optional

### CommandHandler
The CommandHandler must inherit from `ICommandHandler<TCommandInput, TCommandResult>` and implement the `Handle` method.

If you dont disabled the pipelines at startup, MediatR will make sure that your input will be logged and validated and every unhandled exception inside the `Handle` scope will be treated.

``` csharp
public class NewPersonCommandHandler : ICommandHandler<NewPersonCommandInput, NewPersonCommandResult>
{
    private readonly ILogger<NewPersonCommandHandler> _logger;
    private readonly IMediator _mediator;

    public NewPersonCommandHandler(ILogger<NewPersonCommandHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<NewPersonCommandResult> Handle(NewPersonCommandInput request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering person...");

        var personId = Guid.NewGuid();

        await Task.Delay(1000, cancellationToken);
        
        return new NewPersonCommandResult(personId);
    }
}
```

This will keep your controllers nice and clean, you just need to inject the `IMediator` interface:

``` csharp
[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> NewPerson([FromBody] NewPersonCommandInput commandInput)
    {
        var result = await _mediator.Send(commandInput);
       
        return result.IsValid() 
            ? Ok(result)
            : BadRequest(new { result.Errors });
    }
}
```

Success response:

```json
{
  "newPersonId": "ce0e831b-0973-4e1a-9ecc-e8c4429784bd",
  "errors": []
}
```

Error response:

``` json
{
  "errors": [
    "'Name' must not be empty.",
    "'Age' must be greater than or equal to '18'."
  ]
}
```

[Checkout Samples](https://github.com/tuliopaim/EasyCqrs/tree/master/sample/EasyCqrs.Sample/Application)

---
## Queries

A query work in the same way of the Command:

* Create (or not) a result inheriting from `QueryResult`
* Create a Input inheriting from `QueryInput<TQueryResult>`
* Create (or not) a Validator inheriting from `QueryInputValidator<TQueryInput>`
* Create a Handler inheriting from `IQueryHandler<TQueryInput, TQueryResult>`

But in the Query's world you have some helpers:

### ListQueryResult

You may need to retrieve a list of itens, so you can create a result class, and inherit/use the `ListQueryResult<TResult>` that contains a `IEnumerable<TResult>`:

Checkout the [ListQueryResult.cs](https://github.com/tuliopaim/EasyCqrs/blob/master/src/EasyCqrs/Queries/ListQueryResult.cs)

### PagedQueryInput / PagedQueryResult

You may also need to work with pagination in your queries, so you can make use of the `PagedQueryInput` and `PagedQueryResult<TResult>`:

Checkout the [PagedQueryInput.cs](https://github.com/tuliopaim/EasyCqrs/blob/master/src/EasyCqrs/Queries/PagedQueryInput.cs) and [PagedQueryResult.cs](https://github.com/tuliopaim/EasyCqrs/blob/master/src/EasyCqrs/Queries/PagedQueryResult.cs)

Checkout [Sample](https://github.com/tuliopaim/EasyCqrs/blob/master/sample/EasyCqrs.Sample/Application/Queries/GetPeopleQuery/GetPeopleQueryHandler.cs)

---
## Events

A Event work in a fire and forget way.

* Create a Input that inherit from `EventInput`
* Create a handler that inherit from `IEventHandler`

    There is not Validation or Results in Events

Checkout [Sample](https://github.com/tuliopaim/EasyCqrs/blob/master/sample/EasyCqrs.Sample/Application/Events/NewPersonEvent/NewPersonEventHandler.cs)

---
## References

Read more about 
[Cqrs](https://martinfowler.com/bliki/CQRS.html), 
[MediatR](https://github.com/jbogard/MediatR),
[Validation Pipeline Behavior](https://medium.com/tableless/fail-fast-validations-com-pipeline-behavior-no-mediatr-e-asp-net-core-f3854d3c21fa)
