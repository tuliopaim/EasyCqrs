using EasyCqrs.Results;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleFailure(Result result)
    {
        return result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            _ => BadRequest(new { Errors = result.Errors.Select(x => x.Message) })
        };
    }
}
