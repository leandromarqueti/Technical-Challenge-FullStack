using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using TechnicalChallenge.Shared.Exceptions;
using System.Net;
using System.Text.Json;

namespace TechnicalChallenge.API.Middlewares;

public class ExceptionHandlingMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred: {ExceptionMessage}", exception.Message);

        var response = httpContext.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = validationException.Message,
                    Errors = validationException.Errors
                };
                break;

            case NotFoundException notFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = notFoundException.Message
                };
                break;

            case DomainException domainException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = domainException.Message
                };
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Erro Interno: {exception.Message}"
                };
                break;
        }

        await response.WriteAsJsonAsync(errorResponse, cancellationToken);
        return true;
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? Errors { get; set; }
}
