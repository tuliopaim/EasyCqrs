using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
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
       
        return result.IsValid
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpGet(Name = "GetPeople")]
    public async Task<IActionResult> GetPeople([FromQuery] GetPeopleQueryInput queryInput)
    {
        var result = await _mediator.Send(queryInput);
       
        return result.IsValid
            ? Ok(result)
            : BadRequest(result);
    }
}