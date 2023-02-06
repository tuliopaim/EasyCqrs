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
    - [Command](#command-input)
    - [CommandValidator](#command-input-validator)
    - [CommandHandler](#command-handler)
  - [Queries](#queries)
    - [QueryResult](#query-result)
    - [Query](#query-input)
    - [QueryHandler](#query-handler)
    - [QueryListResult](#query-list-result)
    - [QueryPaginatedInput](#query-paginated-input)
    - [QueryPaginatedResult](#query-paginated-result)
  - [Events](#events)
    - [Event](#event-input)
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

- Auto injected Handlers
- Pipelines
  - Validation Pipeline - Auto validate inputs before entering the handler

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
        - NewPersonCommand.cs
        - NewPersonCommandHandler.cs
        - NewPersonCommandValidator.cs
    - UpdatePersonCommand
		- UpdatePersonCommand.cs
    	- UpdatePersonCommandHandler.cs
    	- UpdatePersonCommandValidator.cs	
- Events
    - NewPersonEvent
        - NewPersonEventHandler.cs
        - NewPersonEvent.cs
- Queries
    - GetPersonByIdQuery
        - GetPersonByIdQuery.cs
        - GetPersonByIdQueryHandler.cs
        - GetPersonByIdItem.cs
    - GetPeoplePaginatedQuery
        - GetPeopleQueryPaginated.cs
        - GetPeopleQueryPaginatedHandler.cs
        - GetPeopleQueryPaginatedItem.cs

--- 

## Usage

### Registering

You can use the `AddCqrs` extension method to inject and configure
the required services in the DI container, passing the Assemblies where the CQRS classes are located (inputs, results, validators and handlers).

``` csharp
builder.Services.AddCqrs(typeof(NewPersonCommandHandler).Assembly);
```

---

## Commands

Each command scope are composed with:

- Command
- CommandValidator
- CommandHandler

### Command

The Command express the input of your command, it's required to implement a specific command input for each command, because it is
used by the MeditR to mediate your Command.
You must create a Command Input class by implementing the `ICommand<TCommandResult>`, where the `TCommandResult` is the result class.

```csharp
public record NewPersonCommand(string? Name, string? Email, int Age) :
    ICommand<Guid>
{
}
```

### Command Validator

You can also create an Input Validator using FluentValidation, it will be used in the ValidatonPipeline to automatically validate your command input.

No extra configuration is required, you just need to create the class inheriting from `AbstractValidator<TCommand>`.

The validatior class is optional.

``` csharp
public class NewPersonCommandValidator : AbstractValidator<NewPersonCommand>
{
    public NewPersonCommandValidator()
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

Your CommandHandler must implement `ICommandHandler<TCommand, TCommandResult>`, `TCommand` been your specific command input and `TCommandResult` your command result, specific or not.

You must implement the abstract `Handle` method, this is the method that MediatR will call when you send a Command

``` csharp
public class NewPersonCommandHandler : ICommandHandler<NewPersonCommand, Guid>
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

    public async Task<Result<Guid>> Handle(NewPersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Person(request.Name!, request.Age);

        _personRepository.AddPerson(person);

        await _mediator.Publish(new NewPersonEvent { PersonId = person.Id }, cancellationToken);

        return Result.Success(person.Id);
    }
}
```

---

## Queries

The queries follows the same struct as the commands, you have a input, a handler, a result and a validator.

- Returns a single object: `IQuery<TItem>`
- Returns a list of objects: `IQuery<IEnumerable<TItem>>`
- Returns a paginated list of objects: `IQuery<PaginatedList<TItem>>`

The input must implement `IQuery<TItem>` and may carry filters or any information required to return the result(s).

The queries scope is similar to the command's scope:

- Query
- QueryValidator
- QueryHandler
- QueryItem

### Query

You must create a query input class by implementing `IQuery<TQueryResult>`, where the `TQueryResult` is your query result class.

``` csharp
public record GetPersonByIdQuery(Guid Id) : IQuery<GetPersonByIdQueryItem>
{
}
```

### Query Handler

The query handler must implement `IQueryHandler<TQuery, TQueryResult>`, TQuery been your specific query input and TQueryResult your query result, specific or not.

You must implement the abstract Handle method, this is the method that MediatR will call when you send a Query

``` csharp
public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQuery, GetPersonByIdQueryItem>
{
    private readonly IPersonRepository _personRepository;

    public GetPersonByIdQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }
    
    public async Task<Result<GetPersonByIdQueryItem>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = _personRepository.GetPeople().FirstOrDefault(x => x.Id == request.Id);
        
        var personResult = person is null
            ? null
            : new GetPersonByIdQueryItem(person.Id, person.Name, person.Age);

        return personResult; // using the implict operator to return 
    }
}
```

## Events

Events works in a fire and forget way.

- Create a Input that implements `IEvent`
- Create a handler that implements from `IEventHandler`

>There is not Validation or Results in Events

### Event Input

``` csharp
public record NewPersonEvent(Guid PersonId) : IEvent
{
}
```

### Event Handler

``` csharp
public class NewPersonEventHandler : IEventHandler<NewPersonEvent>
{
    private readonly ILogger<NewPersonEventHandler> _logger;

    public NewPersonEventHandler(ILogger<NewPersonEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewPersonEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Person [{PersonId}] created!", notification.PersonId);

        return Task.CompletedTask;
    }
}
```

## Pipelines

Pipeline behaviors is a way that MeditR give us to insert code into the pipeline.

When we call `IMediator.Send(new FooCommand())`, the input will pass throught
all the pipelines until it get into the Handler method, and then will return throught them again. 

For example, today EasyCqrs has the ValidationPipelineBehavior (you also can create yours):

    Controller => ValidationPipeline => Handler

and the return
    
    Controller <= ValidationPipeline <= Handler


[Read more about MediatR Pipeline Behavior](https://codewithmukesh.com/blog/mediatr-pipeline-behaviour/)


### Validation Pipeline

The Validation Pipeline is responsible for retreive all the validators for that input from the DI container,
validate the input, and notifies the errors with the INotifier interface.

Meaning that if the input has any validation errors, the request will short circuit and return to the caller.
