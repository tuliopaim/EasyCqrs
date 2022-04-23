using EasyCqrs.Mvc;
using EasyCqrs.Sample.Application.Commands.ExceptionThrownCommand;
using EasyCqrs.Sample.Application.Queries.ExceptionThrownQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class ExceptionThrownController : CqrsController
{
    private readonly IMediator _mediator;

    public ExceptionThrownController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Name = nameof(PostThrowException))]
    public async Task<IActionResult> PostThrowException ()
    {
        var result = await _mediator.Send(new ExceptionThrownCommandInput());

        return HandleResult(result);
    }

    [HttpGet(Name = nameof(GetThrowException))]
    public async Task<IActionResult> GetThrowException()
    {
        var result = await _mediator.Send(new ExceptionThrownQueryInput());

        return HandleResult(result);
    }
}
    