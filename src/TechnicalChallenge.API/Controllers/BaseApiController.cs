using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TechnicalChallenge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            //pega o id do usuário nos claims do token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return idClaim != null ? Guid.Parse(idClaim) : Guid.Empty;
        }
    }
}
