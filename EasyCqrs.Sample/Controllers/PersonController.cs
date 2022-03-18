using EasyCqrs.Mediator;
using EasyCqrs.Sample.Commands.NewPersonCommand;
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
       
        return result.IsValid() 
            ? Ok(result)
            : BadRequest(result);
    }
}