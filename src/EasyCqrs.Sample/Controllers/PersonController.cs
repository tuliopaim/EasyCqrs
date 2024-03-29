using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Commands.UpdatePersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeopleByAgeQuery;
using EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;
using EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : BaseController
{
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Name = nameof(NewPerson))]
    public async Task<IActionResult> NewPerson([FromBody] NewPersonCommand commandInput)
    {
        var result = await _mediator.Send(commandInput);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetPersonById), new { Id = result.Value }, result.Value)
            : HandleFailure(result);
    }

    [HttpPut(Name = nameof(UpdatePerson))]
    public async Task<IActionResult> UpdatePerson([FromBody] UpdatePersonCommand commandInput)
    {
        var result = await _mediator.Send(commandInput);

        return result.IsSuccess 
            ? Ok()
            : HandleFailure(result);
    }

    [HttpGet("{id:guid}", Name = nameof(GetPersonById))]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var result = await _mediator.Send(new GetPersonByIdQuery(id));

        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpGet("paginated", Name = "GetPeoplePaginated")]
    public async Task<IActionResult> GetPeoplePaginated([FromQuery] GetPeopleQueryPaginated input)
    {
        var result = await _mediator.Send(input);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }

    [HttpGet("{age:int}", Name = "GetPeopleByAge")]
    public async Task<IActionResult> GetPeopleByAge(int age)
    {
        var result = await _mediator.Send(new GetPeopleByAgeQuery(age)); ;

        return result.IsSuccess 
            ? Ok(result.Value) 
            : HandleFailure(result);
    }
}
