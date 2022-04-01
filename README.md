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

* Auto injected INotificator
* Auto injected Handlers
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

## Notificator

You can use the injected `INotificator` interface to gather error messages across the scope:

```csharp
public class SomeService
{
    private readonly INotificator _notificator;

    public SomeService(INotificator notificator)
    {
        _notificator = notificator;
    }

    public async Task SomeProcessingMethod()
    {
        //...

        if (SomethingIsWrong(foo, bar))
        {
            _notificator.AddNotification("Something is wrong!");
            return;
        }
        
        //...
    }
}
```

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
The CommandHandler is where your orchestration logic will be created, you could have calls to services, repositories and basicly anything that you need to do in order to complete your command mission.

Your CommandHandler must inherit from `ICommandHandler<TCommandInput, TCommandResult>`, `TCommandInput` been your specific CommandInput, `TCommandResult` been your CommandResult, specific or not.

You must implement the abstract `Handle` method, this is the method that MediatR will call when you send a CommandInput

If you dont disabled the pipelines at startup, MediatR will make sure that your CommandInput will be logged and validated and every unhandled exception inside the `Handle` scope will be treated.

``` csharp
public class NewPersonCommandHandler : ICommandHandler<NewPersonCommandInput, NewPersonCommandResult>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMediator _mediator;

    public NewPersonCommandHandler(
        IPersonRepository personRepository,
        IMediator mediator)
    {
        _personRepository = personRepository;
        _mediator = mediator;
    }

    public async Task<NewPersonCommandResult> Handle(NewPersonCommandInput request, CancellationToken cancellationToken)
    {
        var person = new Person(request.Name!, request.Age);

        _personRepository.AddPerson(person);

        await _mediator.Publish(new NewPersonEventInput { PersonId = person.Id }, cancellationToken);

        return new NewPersonCommandResult { Id = person.Id };
    }
}
```
---

## Queries

Each query must retriave a result. Period. EasyCqrs provides the base classes needed to retrieve:

* A single object: `QueryResult<TResult>`
* A list of objects: `QueryListResult<TResult>` 
* A paginated list of objects: `QueryPaginatedResult<TResult>` 

The inputs must inherit from `QueryInput<TQueryResult>` or `QueryPaginatedInput<TQueryResult>`, and can carry filters or any information required to return the result(s). 

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
public class GetPersonByIdQueryResult : QueryResult<GetPersonByIdResult> { }
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
    private readonly IPersonRepository _personRepository;

    public GetPersonByIdQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    public Task<QueryResult<GetPersonByIdResult>> Handle(GetPersonByIdQueryInput request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPeople().FirstOrDefault(x => x.Id == request.Id);
        
        var personResult = GetPersonByIdResult.FromPerson(person);

        return Task.FromResult(new QueryResult<GetPersonByIdResult>
        {
            Result = personResult
        });
    }
}
```

### QueryListResult

The `QueryListResult<TQueryResult>` helper class has a `IEnumerable<TResult>` as Result, and can be used if you need to retreive a list of objects:

```csharp
// QueryListResult base class
public class QueryListResult<TResult> : QueryResult<IEnumerable<TResult>>
{
}

// you could also do this
public class FooQueryResult : QueryListResult<TResult> { }
```


### QueryPaginatedInput

The `QueryPaginatedInput<TQueryResult>` helper class contains a `PageSize` and `PageNumber` properties. You can inherit from it and use any other custom filter properities you need.. 

``` csharp
public class GetPeopleQueryPaginatedInput : QueryPaginatedInput<GetPeopleQueryPaginatedResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}

public class GetPeopleQueryPaginatedInputValidator : QueryInputValidator<GetPeopleQueryPaginatedInput>
{
    public GetPeopleQueryPaginatedInputValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
```
### QueryPaginatedResult

The `QueryPaginatedResult<TResult>` inherit from `QueryListResult<TResult>`, witch means that it has a `IEnumerable<TResult>` as Result, but also a `QueryPagination` property, with pagination realted information.

```csharp
// QueryPaginatedResult base class
public class QueryPaginatedResult<TResult> : QueryListResult<TResult>
{
    public QueryPagination Pagination { get; set; } = new();
}

//you could also do this
public class GetPeopleQueryPaginatedResult : QueryPaginatedResult<GetPeopleResult> { }
```

Pagination Handler example:

``` csharp
public class GetPeopleQueryPaginatedHandler : IQueryHandler<GetPeopleQueryPaginatedInput, GetPeopleQueryPaginatedResult>
{
    private readonly IPersonRepository _repository;

    public GetPeopleQueryPaginatedHandler(IPersonRepository repository)
    {
        _repository = repository;
    }

    public Task<GetPeopleQueryPaginatedResult> Handle(GetPeopleQueryPaginatedInput request, CancellationToken cancellationToken)
    {
        var filteredData = GetFilteredPeople(request);
        
        var total = filteredData.Count();
        
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

        return Task.FromResult(new GetPeopleQueryPaginatedResult
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

    private IQueryable<Person> GetFilteredPeople(GetPeopleQueryPaginatedInput request)
    {
        var filteredData = _repository.GetPeople();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filteredData = filteredData.Where(x => x.Name.Contains(request.Name));
        }

        if (request.Age != default)
        {
            filteredData = filteredData.Where(x => x.Age == request.Age);
        }

        return filteredData;
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
    public async Task<IActionResult> GetPeoplePaginated([FromQuery] GetPeopleQueryPaginatedInput QueryPaginatedInput)
    {
        var result = await _mediator.Send(QueryPaginatedInput);

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
    public async Task<IActionResult> GetPeoplePaginated([FromQuery] GetPeopleQueryPaginatedInput QueryPaginatedInput)
    {
        var result = await _mediator.Send(QueryPaginatedInput);

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
  "id": "26036708-8d3d-4fa3-81c8-a391f70131c0",
  "isValid": true,
  "errors": []
}
```

GetPersonById Success response:

``` json
{
  "result": {
    "id": "26036708-8d3d-4fa3-81c8-a391f70131c0",
    "name": "Person Y",
    "age": 69
  },
  "isValid": true,
  "errors": []
}
```

GetPeoplePaginated Success response:

``` json
{
  "pagination": {
    "totalElements": 4,
    "pageSize": 2,
    "pageNumber": 0,
    "totalPages": 2,
    "firstPage": 0,
    "lastPage": 1,
    "hasPrevPage": false,
    "hasNextPage": true,
    "prevPage": 0,
    "nextPage": 1
  },
  "result": [
    {
      "id": "60e7c9cf-f7c2-41e4-b49b-adc4856fd097",
      "name": "Person W",
      "age": 40
    },
    {
      "id": "4d91d905-da80-4da1-ae22-bf7cc79ba9df",
      "name": "Person X",
      "age": 22
    }
  ],
  "isValid": true,
  "errors": []
}
```
