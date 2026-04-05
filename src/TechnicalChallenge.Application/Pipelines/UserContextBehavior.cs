using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TechnicalChallenge.Application.Common.Interfaces;

namespace TechnicalChallenge.Application.Pipelines;

public class UserContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IUserOwnedRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            request.UserId = userId;
        }

        return await next();
    }
}
