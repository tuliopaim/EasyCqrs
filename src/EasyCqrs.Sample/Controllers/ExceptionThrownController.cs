using EasyCqrs.Mvc;
using EasyCqrs.Sample.Application.Commands.DivideByZeroCommand;
using EasyCqrs.Sample.Application.Queries.DivideByZeroQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class DivideByZeroController : CqrsController
{
    private readonly IMediator _mediator;

    public DivideByZeroController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Name = nameof(PostThrowException))]
    public async Task<IActionResult> PostThrowException ()
    {
        var result = await _mediator.Send(new DivideByZeroCommandInput());

        return HandleResult(result);
    }

    [HttpGet(Name = nameof(GetThrowException))]
    public async Task<IActionResult> GetThrowException()
    {
        var result = await _mediator.Send(new DivideByZeroQueryInput());

        return HandleResult(result);
    }
}
    