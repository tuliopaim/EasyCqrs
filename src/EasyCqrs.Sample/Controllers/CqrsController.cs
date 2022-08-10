using EasyCqrs.Notifications;
using EasyCqrs.Queries;
using EasyCqrs.Sample.Application.Commands.Common;
using Microsoft.AspNetCore.Mvc;

namespace EasyCqrs.Sample.Controllers;

public abstract class CqrsController : Controller
{
    private readonly INotifier _notifier;

    protected CqrsController(INotifier notifier)
    {
        _notifier = notifier;
    }

    protected IActionResult HandleResult<T>(QueryPaginatedResult<T> result)
    {
        if (_notifier.Exception is not null)
        {
            return StatusCode(500);
        }

        if (!_notifier.IsValid) return ErrorsBadRequest();

        return Ok(new
        {
            _notifier.IsValid,
            result.Result,
            result.Pagination
        });
    }

    protected IActionResult HandleResult<T>(QueryResult<T> result)
    {
        if (_notifier.Exception is not null)
        {
            return StatusCode(500);
        }

        if (!_notifier.IsValid) return ErrorsBadRequest();

        return Ok(new
        {
            _notifier.IsValid,
            result.Result,
        });
    }

    protected IActionResult HandleResult(object result = null)
    {
        if (_notifier.Exception is not null)
        {
            return StatusCode(500);
        }

        if (!_notifier.IsValid) return ErrorsBadRequest();

        return Ok(new
        {
            _notifier.IsValid,
            result
        });
    }

    protected IActionResult HandleCreatedResult(string actionName, object routeValues, CreatedCommandResult result)
    {
        if (_notifier.Exception is not null)
        {
            return StatusCode(500);
        }

        if (!_notifier.IsValid) return ErrorsBadRequest();
   
        return CreatedAtAction(actionName, routeValues, new
        {
            _notifier.IsValid,
            result
        });
    }
    
    private IActionResult ErrorsBadRequest()
    {
        return BadRequest(new
        {
            _notifier.IsValid,
            _notifier.Errors
        });
    }


}