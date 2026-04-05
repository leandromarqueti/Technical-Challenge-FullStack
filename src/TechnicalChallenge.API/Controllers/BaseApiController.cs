using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechnicalChallenge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid UserId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}
