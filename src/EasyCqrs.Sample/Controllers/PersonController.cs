using EasyCqrs.Mvc;
using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

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
    public async Task<IActionResult> GetPeoplePaginated([FromQuery] GetPeopleQueryPaginatedInput input)
    {
        var result = await _mediator.Send(input);

        return HandleResult(result);
    }

    [HttpGet("{age:int}", Name = "GetPeopleByAge")]
    public async Task<IActionResult> GetPeopleByAge(int age)
    {
        var result = await _mediator.Send(new GetPeopleByAgeQueryInput { Age = age }); ;

        return HandleResult(result);
    }
}
