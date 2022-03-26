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
## Features

* Serilog
* Auto injected handlers
* Auto injected validators
* Auto inject INotificator
* Exception Pipeline
    * No unhandled exceptions inside Handlers
* Log Pipeline
    * Logs entering and leaving handlers and the input serialized
* Validation Pipeline
    * Auto validate the inputs with the input validators before entering the handler
* Notification Pipeline
    * All the notifications will be added inside the result's Errors

Read more about 
[Cqrs](https://martinfowler.com/bliki/CQRS.html), 
[MediatR](https://github.com/jbogard/MediatR),
[MediatR Pipeline Behavior](https://codewithmukesh.com/blog/mediatr-pipeline-behaviour/)

---
## Usage

### Registering

You can use the AddCqrs extension method on IServiceCollection to inject and configure the required services in the DI container.

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
        config.DisableNotificationPipeline();
    });
```

---

## Commands

Each command scope are composed with:

* CommandResult
* CommandInput
* CommandInputValidator
* CommandHandler 

### CommandResult

The `CommandResult` express what is returned from your `CommandHandler`, inherit from it if you want to return any extra information.
   
> :warning: To validation and exception pipelines work properly your custom command result class **must** have a parameterless contructor, otherwise your handler will not compile.
:warning:

```csharp
public class NewPersonCommandResult : CommandResult
{
    public Guid Id { get; set; }
}     
```
    Specifc CommandResult class are not required, you can use the CommandResult instead.


### CommandInput
You create a Command Input class by inheriting from `CommandInput<TCommandResult>`, where the `TCommandResult` generics will be the CommandResult, for this example, `NewPersonCommandResult`

```csharp
public class NewPersonCommandInput : CommandInput<NewPersonCommandResult>
{
    public NewPersonCommandInput(string? name, int age)
    {
        Name = name;
        Age = age;
    }

    public string? Name { get; set; }
    public int Age { get; set; }
}
```

### CommandInputValidator
You can also create an Input Validator using FluentValidation, it will be used in the ValidatonPipeline to validate the `CommandInput`.

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

    The InputValidatior class is optional

### CommandHandler
The CommandHandler is where your orchestration will be, mapping to entity, calls to Services, Repositories, anything that you need to do in order to complete your command mission.

It must inherit from `ICommandHandler<TCommandInput, TCommandResult>`, `TCommandInput` been your specific CommandInput, `TCommandResult` been your CommandResult, specific or not.

You must implement the abstract `Handle` method, this is the method that MediatR will call when you send a CommandInput

If you dont disabled the pipelines at startup, MediatR will make sure that your CommandInput will be logged and validated and every unhandled exception inside the `Handle` scope will be treated.

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

        await Task.Delay(1000, cancellationToken);

        var person = new Person(request.Name, request.Age);

        await _mediator.Publish(new NewPersonEventInput { PersonId = person.Id }, cancellationToken);

        return new NewPersonCommandResult { Id = person.Id };
    }
}
```
---

## Queries

Each query must retriave a result. Period. EasyCqrs provides the base classes needed to retrieve:

* A single object: `QueryResult<TResult>`
* A list of objects: `ListQueryResult<TResult>` 
* A paginated list of objects: `PaginatedQueryResult<TResult>` 

The inputs must inherit from `QueryInput<TQueryResult>` or `PaginatedQueryInput<TQueryResult>`, and can carry filters or any information required to return the result(s). 

The queries scope works like the Command's scope:

* A result class
* An input class
* An input validator class
* A query handler

### QueryResult

You can either create a SpecificQueryResult inheriting from `QueryResult<TResult>` class or use it directly, in this example i will use 
`QueryResult<GetPersonByIdResult>` Directly

``` csharp
// QueryResult base class
public class QueryResult<TResult> : QueryResult
{
    public TResult? Result { get; set; }
}

// you could also do this
public class FooQueryResult : QueryResult<TResult> { }
```

### QueryInput

``` csharp
public class GetPersonByIdQueryInput : QueryInput<QueryResult<GetPersonByIdResult>>
{
    public GetPersonByIdQueryInput(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
```

### QueryHandler

``` csharp
public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQueryInput, QueryResult<GetPersonByIdResult>>
{
    public Task<QueryResult<GetPersonByIdResult>> Handle(GetPersonByIdQueryInput request, CancellationToken cancellationToken)
    {
        //get the result from your data source...

        var personResult = new GetPersonByIdResult(request.Id, "Person 1", 24);

        return Task.FromResult(new QueryResult<GetPersonByIdResult>
        {
            Result = personResult
        });
    }
}
```

### ListQueryResult

The `ListQueryResult<TQueryResult>` helper class has a `IEnumerable<TResult>` as Result, and can be used if you need to retreive a list of objects:

```csharp
// ListQueryResult base class
public class ListQueryResult<TResult> : QueryResult<IEnumerable<TResult>>
{
}

// you could also do this
public class FooQueryResult : ListQueryResult<TResult> { }
```


### PaginatedQueryInput

The `PaginatedQueryInput<TQueryResult>` helper class contains a `PageSize` and `PageNumber` properties. You can inherit from it and use any other custom filter properities you need.. 

``` csharp
public class GetPeoplePaginatedQueryInput : PaginatedQueryInput<GetPeoplePaginatedQueryResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}

public class GetPeoplePaginatedQueryInputValidator : QueryInputValidator<GetPeoplePaginatedQueryInput>
{
    public GetPeoplePaginatedQueryInputValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
```
### PaginatedQueryResult

The `PaginatedQueryResult<TResult>` inherit from `ListQueryResult<TResult>`, witch means that it has a `IEnumerable<TResult>` as Result, but also a `QueryPagination` property, with pagination realted information.

```csharp
// PaginatedQueryResult base class
public class PaginatedQueryResult<TResult> : ListQueryResult<TResult>
{
    public QueryPagination Pagination { get; set; } = new();
}

//you could also do this
public class GetPeoplePaginatedQueryResult : PaginatedQueryResult<GetPeopleResult> { }
```

Pagination Handler example:

``` csharp
public class GetPeoplePaginatedQueryHandler : IQueryHandler<GetPeoplePaginatedQueryInput, GetPeoplePaginatedQueryResult>
{
    public Task<GetPeoplePaginatedQueryResult> Handle(GetPeoplePaginatedQueryInput request, CancellationToken cancellationToken)
    {
        var filteredData = GetPersons();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filteredData = filteredData.Where(x => x.Name.Contains(request.Name));
        }

        if (request.Age != default)
        {
            filteredData = filteredData.Where(x => x.Age == request.Age);
        }

        // retreive your total filtered data count from your data source...
        var total = filteredData.Count();

        // retreive your paginated data from your data source...
        var paginatedResult = filteredData
            .OrderBy(x => x.Name)
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetPeopleResult
            {
                Id = x.Id,
                Name = x.Name,
                Age = x.Age,
            }).ToList();

        return Task.FromResult(new GetPeoplePaginatedQueryResult
        {
            Result = paginatedResult,
            Pagination = new QueryPagination
            {
                PageNumber = request.PageNumber,
                PageSize = paginatedResult.Count,
                TotalElements = total
            }
        });
    }

    private static IQueryable<Person> GetPersons()
    {
        var list = new List<Person>(20);

        for (var i = 1; i <= 20; i++)
        {
            list.Add(new Person($"Person {i:D2}", new Random().Next(20, 90)));
        }

        return list.AsQueryable();
    }
}
```

## Events

Events works in a fire and forget way.

* Create a Input that inherit from `EventInput`
* Create a handler that inherit from `IEventHandler`

    There is not Validation or Results in Events

### EventInput

``` csharp
public class NewPersonEventInput : EventInput
{
    public Guid PersonId { get; set; }
}
```

### EventHandler

``` csharp
public class NewPersonEventHandler : IEventHandler<NewPersonEventInput>
{
    private readonly ILogger<NewPersonEventHandler> _logger;

    public NewPersonEventHandler(ILogger<NewPersonEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewPersonEventInput notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Person [{PersonId}] created!", notification.PersonId);

        return Task.CompletedTask;
    }
}
```

---

## Controllers

EasyCqrs will keep your controllers nice and clean, you just need to inject and use the `IMediator` interface, without worring about logging, validation and unhandled exceptions:

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
    
    [HttpGet("{id:guid}", Name = "GetPersonById")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var result = await _mediator.Send(new GetPersonByIdQueryInput(id)); 

        return result.IsValid() 
            ? Ok(result)
            : BadRequest(new { result.Errors });
    }

    [HttpGet("paginated", Name = "GetPeoplePaginated")]
    public async Task<IActionResult> GetPeoplePaginated([FromQuery] GetPeoplePaginatedQueryInput paginatedQueryInput)
    {
        var result = await _mediator.Send(paginatedQueryInput);

        return result.IsValid() 
            ? Ok(result)
            : BadRequest(new { result.Errors });
    }
}
```

You can also use the `CqrsController`, in order to use the `HandleResult` method:

``` csharp
[ApiController]
[Route("[controller]")]
public class PersonController : CqrsController
{
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Name = "NewPerson")]
    public async Task<IActionResult> NewPerson([FromBody] NewPersonCommandInput commandInput)
    {
        var result = await _mediator.Send(commandInput);
       
        return HandleResult(result);
    }

    [HttpGet("{id:guid}", Name = "GetPersonById")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var result = await _mediator.Send(new GetPersonByIdQueryInput(id)); 

        return HandleResult(result);
    }

    [HttpGet("paginated", Name = "GetPeoplePaginated")]
    public async Task<IActionResult> GetPeoplePaginated([FromQuery] GetPeoplePaginatedQueryInput paginatedQueryInput)
    {
        var result = await _mediator.Send(paginatedQueryInput);

        return HandleResult(result);
    }
}
```

Error response:

``` json
{
  "isValid": false,
  "errors": [
    "'Name' must not be empty.",
    "'Age' must be greater than or equal to '18'."
  ]
}
```

NewPerson Success response:

```json
{
  "id": "58093d52-1dde-4da1-8947-91e148934862",
  "isValid": true,
  "errors": []
}
```

GetPersonById Success response:

``` json
{
  "result": {
    "id": "58093d52-1dde-4da1-8947-91e148934862",
    "name": "Person 1",
    "age": 24
  },
  "isValid": true,
  "errors": []
}
```

GetPeoplePaginated Success response:

``` json
{
  "pagination": {
    "totalElements": 20,
    "pageSize": 2,
    "pageNumber": 3,
    "totalPages": 10,
    "firstPage": 0,
    "lastPage": 9,
    "hasPrevPage": true,
    "hasNextPage": true,
    "prevPage": 2,
    "nextPage": 4
  },
  "result": [
    {
      "id": "2727c295-ead6-4b39-8a9e-99a51d433d23",
      "name": "Person 07",
      "age": 59
    },
    {
      "id": "1dcf3091-c3f2-4e49-ac9d-74ea4db83606",
      "name": "Person 08",
      "age": 21
    }
  ],
  "isValid": true,
  "errors": []
}
```
