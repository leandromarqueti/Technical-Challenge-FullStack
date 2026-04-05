using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechnicalChallenge.Application.UseCases.Dashboard.Queries.GetDashboard;

using Microsoft.AspNetCore.Authorization;

namespace TechnicalChallenge.API.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardQuery { UserId = UserId }, cancellationToken);
        return Ok(result);
    }
}
