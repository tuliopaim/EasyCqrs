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
    - [CommandResult](#command-result)
    - [CommandInput](#command-input)
    - [CommandInputValidator](#command-input-validator)
    - [CommandHandler](#command-handler)
  - [Queries](#queries)
    - [QueryResult](#query-result)
    - [QueryInput](#query-input)
    - [QueryHandler](#query-handler)
    - [QueryListResult](#query-list-result)
    - [QueryPaginatedInput](#query-paginated-input)
    - [QueryPaginatedResult](#query-paginated-result)
  - [Events](#events)
    - [EventInput](#event-input)
    - [EventHandler](#event-handler)
  - [Pipelines](#pipelines)
    - [Log Pipeline](#log-pipeline)
	- [Validation Pipeline](#validation-pipeline)

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
    - Customizable log the input and response before and after handle execution
  - Validation Pipeline
    - Auto validate inputs before entering the handler
- Exception Middleware
  - Middleware ready to register to log unhlanded exceptions

Read more about
[Cqrs](https://martinfowler.com/bliki/CQRS.html)
[MediatR](https://github.com/jbogard/MediatR)
[MediatR Pipeline Behavior](https://codewithmukesh.com/blog/mediatr-pipeline-behaviour/)

---

## Basic Concepts

The main idea is that you can create an application and setup to work with CQRS very easly.

You can structure the application in such a way that all classes related to that specific command or query or event are in the same directory, for example:

- Commands
    - NewPersonCommand
        - NewPersonCommandHandler.cs
        - NewPersonCommandInput.cs
        - NewPersonCommandInputValidator.cs
    - UpdatePersonCommand
    	- UpdatePersonCommandHandler.cs
		- UpdatePersonCommandInput.cs
    	- UpdatePersonCommandInputValidator.cs	
- Queries
    - GetPersonByIdQuery
        - GetPersonByIdQueryHandler.cs
        - GetPersonByIdQueryInput.cs
        - GetPersonByIdQueryInputValidator.cs
        - GetPersonByIdItem.cs
    - GetPeoplePaginatedQuery
        - GetPeopleQueryPaginatedHandler.cs
        - GetPeopleQueryPaginatedInput.cs
        - GetPeopleQueryPaginatedInputValidator.cs
        - GetPeopleQueryPaginatedItem.cs
        - GetPeopleQueryPaginatedResult.cs
- Events
    - NewPersonEvent
        - NewPersonEventHandler.cs
        - NewPersonEventInput.cs

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

You can use the `INotifier` interface to notify errors across the scope, the interface is used to notify the validation errors in the validation pipeline.

It's possible to create your own implementation of INotifier and inject as scoped.

```csharp
public class UpdatePersonCommandHandler : ICommandHandler<UpdatePersonCommandInput, CommandResult>
{
    private readonly IPersonRepository _personRepository;
    private readonly INotifier _notifier;

    public UpdatePersonCommandHandler(
        IPersonRepository personRepository,
        INotifier notifier)
    {
        _personRepository = personRepository;
        _notifier = notifier;
    }

    public async Task<CommandResult> Handle(UpdatePersonCommandInput request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPersonById(request.Id);

        if (person is null)
        {
            _notifier.Notify("Person not found!");
            return new();
        }

        person.Update(request.Name!, request.Email!, request.Age);

        _personRepository.UpdatePerson(person);

        return new();
    }
}
```

## Commands

Each command scope are composed with:

- CommandResult
- CommandInput
- CommandInputValidator
- CommandHandler

### Command Result

A Command Result express what is returned from your `CommandHandler`. 
You can implement your own result especific to your command, or use the `CommandResult` if you dont need to return nothing.

> :warning: To validation and exception pipeline work properly your custom command result class **must** have a public parameterless contructor, otherwise your handler will not compile. :warning:

```csharp
public class CreatedCommandResult
{
    public Guid Id { get; set; }
}     
```

### Command Input

The Command Input express the input of your command, it's required to implement a specific command input for each command, because it is
used by the MeditR to mediate your Command.
You must create a Command Input class by implementing the `ICommandInput<TCommandResult>`, where the `TCommandResult` is the command result class, for this example, `CreatedCommandResult`

```csharp
public record NewPersonCommandInput(string? Name, string? Email, int Age) :
    ICommandInput<CreatedCommandResult>
{
}

public class CreatedCommandResult
{
    public Guid Id { get; set; }
}  
```

### Command Input Validator

You can also create an Input Validator using FluentValidation, it will be used in the ValidatonPipeline to automatically validate your command input.

No extra configuration is required, you just need to create the class inheriting from `CommandInputValidator<TCommandInput>`.

The InputValidatior class is optional.

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
        
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18);
    }
}
```


### Command Handler

The Command Handler is where your orchestration logic will be created, you could have calls to services, repositories and basicly anything that you need to do in order to complete your command.

Your CommandHandler must implement `ICommandHandler<TCommandInput, TCommandResult>`, `TCommandInput` been your specific command input and `TCommandResult` your command result, specific or not.

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

The queries follows the same struct as the commands, you have a input, a handler, a result and a validator.

Each query must retriave a result. EasyCqrs provides the base classes needed for you use as your query result

- A single object: `QueryResult<TItem>`
- A list of objects: `QueryListResult<TItem>`
- A paginated list of objects: `QueryPaginatedResult<TItem>`

The inputs must implement `IQueryInput<TQueryResult>` and can carry filters or any information required to return the result(s).

The queries scope is similar to the command's scope:

- QueryResult
- QueryInput
- QueryInputValidator
- QueryHandler
- QueryItem

### Query Result

You can either create a specific query result inheriting from `QueryResult<TItem>` class or use it directly.

``` csharp
public class GetPersonByIdQueryResult : QueryResult<GetPersonByIdQueryItem> { }

public class GetPersonByIdQueryItem
{
    public string Email { get; set; }
    public int Age { get; set; }
}
```
The QueryResult class also has a implicit operator to TItem:

``` csharp
public class QueryResult<TItem>
{
    public TItem? Result { get; set; }

    public static implicit operator QueryResult<TItem>(TItem? result)
    {
        return new QueryResult<TItem> { Result = result };
    }
}
```

I usually prefer to use the QueryResult classes direct instead of creating a empty one.

> :warning: Just like the command result, to validation and exception pipeline work properly your custom query result class **must** have a public parameterless contructor, otherwise your handler will not compile. :warning:

### Query Input

You must create a query input class by implementing `IQueryInput<TQueryResult>`, where the `TQueryResult` is your query result class.

``` csharp
public record GetPersonByIdQueryInput(Guid Id) : IQueryInput<QueryResult<GetPersonByIdQueryItem>>
{
}
```

### Query Handler

The query handler must implement `IQueryHandler<TQueryInput, TQueryResult>`, TQueryInput been your specific query input and TQueryResult your query result, specific or not.

You must implement the abstract Handle method, this is the method that MediatR will call when you send a QueryInput

``` csharp
public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQueryInput, QueryResult<GetPersonByIdQueryItem>>
{
    private readonly IPersonRepository _personRepository;

    public GetPersonByIdQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    
    public async Task<QueryResult<GetPersonByIdQueryItem>> Handle(GetPersonByIdQueryInput request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPeople().FirstOrDefault(x => x.Id == request.Id);
        
        var personResult = person is null
            ? null
            : new GetPersonByIdQueryItem(person.Id, person.Name, person.Age);

        return personResult; // using the implict operator to return 
    }
}
```

### Query List Result

The `QueryListResult<TQueryResult>` helper class has a `IEnumerable<TItem>` as Result, and can be used if you need to retreive a IEnumerable of objects.

```csharp
public class QueryListResult<TItem> : QueryResult<IEnumerable<TItem>>
{
}
```

Usage:

``` csharp
public record GetPeopleByAgeQueryInput(int Age)
    : IQueryInput<QueryListResult<GetPeopleByAgeItem>>
{
}
```

### Query Paginated Input

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

### Query Paginated Result

The `QueryPaginatedResult<TItem>` inherit from `QueryListResult<TItem>`, witch means that it has a `IEnumerable<TItem>` as Result, but also a `QueryPagination` property, with pagination realted information.

```csharp
public class QueryPaginatedResult<TItem> : QueryListResult<TItem>
{
    public QueryPagination Pagination { get; set; } = new();
}
```

Usage:

``` csharp
public class GetPeopleQueryPaginatedResult : QueryPaginatedResult<GetPeopleQueryPaginatedItem> { }

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

### Event Input

``` csharp
public class NewPersonEventInput : EventInput
{
    public Guid PersonId { get; set; }
}
```

### Event Handler

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

## Pipelines

Pipeline behaviors is a way that MeditR give us to insert code into the pipeline.

When we call `IMediator.Send(new FooCommandInput())`, the input will pass throught
all the pipelines until it get into the Handler method, and then will return throught them again. 

For example, today EasyCqrs has the LogPipelineBehavior and the ValidationPipelineBehavior (you also can create yours):

Mediator.Send => LogPipeline => ValidationPipeline => Handler

and the return
    
Mediator.Send <= LogPipeline <= ValidationPipeline <= Handler


[Read more about MediatR Pipeline Behavior](https://codewithmukesh.com/blog/mediatr-pipeline-behaviour/)


### Log Pipeline

The LogPipeline uses the `IPipelineLogService` to perform a log before and after the execution:

``` csharp
public class LogPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMediatorInput<TResponse>
{
    private readonly IPipelineLogService _pipelineLogService;

    public LogPipelineBehavior(IPipelineLogService pipelineLogService)
    {
        _pipelineLogService = pipelineLogService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        await _pipelineLogService.LogBeforeAsync(request, cancellationToken);

        var response = await next();

        await _pipelineLogService.LogAfterAsync(request, response, cancellationToken);

        return response;
    }
}
```

The default PipelineLogService just LogDebug the type of the request, but you can always implement your own and inject as Scoped:

``` csharp
public class PipelineLogService : IPipelineLogService
{
    private readonly ILogger<PipelineLogService> _logger;

    public PipelineLogService(ILogger<PipelineLogService> logger)
    {
        _logger = logger;
    }

    public Task LogBeforeAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{RequestType} - Entering handler!", typeof(TRequest).Name);
        return Task.CompletedTask;
    }

    public Task LogAfterAsync<TRequest, TResponse>(TRequest request, TResponse? response, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{RequestType} - Leaving handler!", typeof(TRequest).Name);
        return Task.CompletedTask;
    }
}
```

### Validation Pipeline

The Validation Pipeline is responsible to retreve all the validators for that input from the DI container,
validate, and, in case of some error notify the INotifier with the error message and return.

Meaning that if the Input has any validation errors, the request will short circuit and return to the caller.
