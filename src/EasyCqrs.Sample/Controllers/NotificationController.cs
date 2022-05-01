using EasyCqrs.Mvc;
using EasyCqrs.Sample.Application.Commands.NotificationCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : CqrsController
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Name = "NewNotification")]
    public async Task<IActionResult> NewNotification([FromBody] NotificationCommandInput commandInput)
    {
        var result = await _mediator.Send(commandInput);
       
        return HandleResult(result);
    }
}
    