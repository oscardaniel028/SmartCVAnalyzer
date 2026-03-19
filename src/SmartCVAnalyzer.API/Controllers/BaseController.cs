using Microsoft.AspNetCore.Mvc;
using SmartCVAnalyzer.Application.Common;
using System.Security.Claims;

namespace SmartCVAnalyzer.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    // Extrae el UserId del JWT token del request actual
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub");

            return Guid.TryParse(sub, out var userId)
                ? userId
                : Guid.Empty;
        }
    }

    // Convierte un Result<T> en la respuesta HTTP adecuada
    protected IActionResult HandleResult<T>(Result<T> result, int successStatusCode = 200)
    {
        if (result.IsFailure)
            return BadRequest(new { message = result.Error });

        return successStatusCode switch
        {
            201 => StatusCode(201, result.Value),
            _ => Ok(result.Value)
        };
    }
}