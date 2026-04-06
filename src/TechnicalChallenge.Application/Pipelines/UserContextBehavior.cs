using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TechnicalChallenge.Application.Common.Interfaces;

namespace TechnicalChallenge.Application.Pipelines;

public class UserContextBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IUserOwnedRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            request.UserId = userId;
        }

        return await next();
    }
}
