using CrookedNumbers.Api.Integrations.Mlb;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CrookedNumbers.Api.Infrastructure;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ApiExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (
            exception is OperationCanceledException
            && httpContext.RequestAborted.IsCancellationRequested
        )
        {
            logger.LogDebug("Request was cancelled by the client.");
            httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            return true;
        }

        var (status, title, detail) = exception switch
        {
            MlbNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message
            ),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                exception.Message
            ),
            MlbApiException => (
                StatusCodes.Status502BadGateway,
                "MLB service error",
                "The upstream baseball data service could not complete the request."
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred."
            ),
        };

        if (status >= 500)
        {
            logger.LogError(exception, "Request failed with status {StatusCode}", status);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail,
                    Instance = httpContext.Request.Path,
                },
                Exception = exception,
            }
        );
    }
}
