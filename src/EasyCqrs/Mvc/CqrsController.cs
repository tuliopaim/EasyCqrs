using EasyCqrs.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Mvc;

public abstract class CqrsController : Controller
{
    protected IActionResult HandleResult(IMediatorResult result)
    {
        if (result.Exception is not null)
        {
            return StatusCode(500);
        }

        return result.IsValid
            ? Ok(result)
            : BadRequest(new { result.IsValid, result.Errors });
    }
}