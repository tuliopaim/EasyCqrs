# EasyCqrs

A library to work easier with CQRS on top of MediatR.

## Table of Contents

- [EasyCqrs](#easycqrs)
  - [Table of Contents](#table-of-contents)
  - [Install](#install)
    - [CLI](#cli)
    - [Package Manager Console](#package-manager-console)
  - [Features](#features)
  - [Usage](#usage)
    - [Registering](#registering)
  - [Notifier](#notifier)
  - [Commands](#commands)
    - [CommandResult](#commandresult)
    - [CommandInput](#commandinput)
    - [CommandInputValidator](#commandinputvalidator)
    - [CommandHandler](#commandhandler)
  - [Queries](#queries)
    - [QueryResult](#queryresult)
    - [QueryInput](#queryinput)
    - [QueryHandler](#queryhandler)
    - [QueryListResult](#querylistresult)
    - [QueryPaginatedInput](#querypaginatedinput)
    - [QueryPaginatedResult](#querypaginatedresult)
  - [Events](#events)
    - [EventInput](#eventinput)
    - [EventHandler](#eventhandler)
  - [Controllers](#controllers)
    - [CqrsController](#cqrscontroller)

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

- Auto injected INotifier
- Auto injected Handlers
- Pipelines
  - Log Pipeline Behavior
    - Log's the input on entering, and the moment of leaving the handler
  - Validation Pipeline
    - Auto validate inputs before entering the handler
  - Notification Pipeline
    - All the notifications will be added inside the result's `Errors` list
  - Exception Pipeline
    - No unhandled exceptions inside the the MediatR Handlers
- Exception Middleware
  - Middleware ready to register to log unhlanded exceptions

Read more about
[Cqrs](https://martinfowler.com/bliki/CQRS.html)
[MediatR](https://github.com/jbogard/MediatR)
[MediatR Pipeline Behavior](https://codewithmukesh.com/blog/mediatr-pipeline-behaviour/)

---

## Usage

### Registering

You can use the `AddCqrs` extension method to inject and configure
the required services in the DI container, passing the Assemblies where the CQRS classes are located (inputs, results, validators and handlers).

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
        config.DisableNotificationPipeline();
    });
```

The `ExceptionMiddleware` logs the all unhandled exceptions and returns a 500 status code.

Usage:

``` csharp
//....

app.UseMiddleware<ExceptionMiddleware>();

app.Run();
```

---

## Notifier

You can use the injected `INotifier` interface to gather error messages across the scope:

```csharp
public class SomeService
{
    private readonly INotifier _notifier;

    public SomeService(INotifier notificator)
    {
        _notifier = notificator;
    }

    public async Task SomeProcessingMethod()
    {
        //...

        if (SomethingIsWrong(foo, bar))
        {
            _notifier.AddNotification("Something is wrong!");
            return;
        }
        
        //...
    }
}
```

## Commands

Each command scope are composed with:

- CommandResult
- CommandInput
- CommandInputValidator
- CommandHandler

### CommandResult

The `CommandResult` express what is returned from your `CommandHandler`. Inherit from it if you want to return any extra information.

> :warning: To validation and exception pipeline work properly your custom command result class **must** have a public parameterless contructor, otherwise your handler will not compile. :warning:

```csharp
public class NewPersonCommandResult : CommandResult
{
    public Guid Id { get; set; }
}     
```

Specifc CommandResult class are not required, you can use the CommandResult instead.

### CommandInput

The `CommandResult` express the input of your command. You must create a Command Input class by inheriting from `CommandInput<TCommandResult>`, where the `TCommandResult` is your command result class, for this example, `NewPersonCommandResult`

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

public class NewPersonCommandResult : CommandResult
{
    public Guid Id { get; set; }
}  
```

### CommandInputValidator

You can also create an Input Validator using FluentValidation, it will be used in the ValidatonPipeline to automatically validate your command input.

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

The CommandHandler is where your orchestration logic will be created, you could have calls to services, repositories and basicly anything that you need to do in order to complete your command.

Your CommandHandler must inherit from `ICommandHandler<TCommandInput, TCommandResult>`, `TCommandInput` been your specific command input and `TCommandResult` your command result, specific or not.

You must implement the abstract `Handle` method, this is the method that MediatR will call when you send a CommandInput

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

- A single object: `QueryResult<TItem>`
- A list of objects: `QueryListResult<TItem>`
- A paginated list of objects: `QueryPaginatedResult<TItem>`

The inputs must inherit from `QueryInput<TQueryResult>` or `QueryPaginatedInput<TQueryResult>`, and can carry filters or any information required to return the result(s).

The queries scope is similar to the command's scope:

- QueryResult / QueryListResult / QueryPaginatedResult
- QueryInput / QueryPaginatedInput
- QueryInputValidator
- QueryHandler

### QueryResult

You can either create a specific qyery result inheriting from `QueryResult<TItem>` class or use it directly.

``` csharp

public class GetPersonByIdQueryResult : QueryResult<GetPersonByIdResult> { }

public class GetPersonByIdResult
{
    public string Email { get; set; }
    public int Age { get; set; }
}
```

> :warning: Just like the command result, to validation and exception pipeline work properly your custom query result class **must** have a public parameterless contructor, otherwise your handler will not compile. :warning:

### QueryInput

You must create a query input class by inheriting from `QueryInput<TQueryResult>`, where the `TQueryResult` is your query result class.

``` csharp
public class GetPersonByIdQueryInput : QueryInput<GetPersonByIdQueryResult>
{
    public GetPersonByIdQueryInput(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
```

### QueryHandler

The query handler must inherit from IQueryHandler<TQueryInput, TQueryResult>, TQueryInput been your specific query input and TQueryResult your query result, specific or not.

You must implement the abstract Handle method, this is the method that MediatR will call when you send a QueryInput

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

The `QueryListResult<TQueryResult>` helper class has a `IEnumerable<TItem>` as Result, and can be used if you need to retreive a IEnumerable of objects.

```csharp
public class QueryListResult<TItem> : QueryResult<IEnumerable<TItem>>
{
}
```

Usage:

``` csharp
public class GetPeopleByAgeQueryResult : QueryListResult<GetPeopleByAgeItem> { }
```

### QueryPaginatedInput

The `QueryPaginatedInput<TQueryResult>` helper class contains a `PageSize` and `PageNumber` properties. You can inherit from it and use any other custom filter properities you need..

``` csharp
public abstract class QueryPaginatedInput<TItem> : QueryInput<TItem>
    where TItem : QueryResult
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
```

Usage:

``` csharp
public class GetPeopleQueryPaginatedInput : QueryPaginatedInput<GetPeopleQueryPaginatedResult>
{
    public string? Name { get; set; }
    public int Age { get; set; }
}

// with custom validation
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

The `QueryPaginatedResult<TItem>` inherit from `QueryListResult<TItem>`, witch means that it has a `IEnumerable<TItem>` as Result, but also a `QueryPagination` property, with pagination realted information.

```csharp
public class QueryPaginatedResult<TItem> : QueryListResult<TItem>
{
    public QueryPagination Pagination { get; set; } = new();
}
```

Usage:

``` csharp
public class GetPeopleQueryPaginatedResult : QueryPaginatedResult<GetPeopleResult> 
{
}
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

- Create a Input that inherit from `EventInput`
- Create a handler that inherit from `IEventHandler`

>There is not Validation or Results in Events

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

EasyCqrs will keep your controllers nice and clean, you just need to inject and use the `IMediator` interface.

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

### CqrsController

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

**Validation** error response:

HTTP 400

``` json
{
  "isValid": false,
  "errors": [
    "'Name' must not be empty.",
    "'Age' must be greater than or equal to '18'."
  ]
}
```

**NewPersonCommand** success response:

```json
{
  "id": "26036708-8d3d-4fa3-81c8-a391f70131c0",
  "isValid": true,
  "errors": []
}
```

**GetPersonByIdQuery** Success response:

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

**GetPeopleQueryPaginated** Success response:

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
