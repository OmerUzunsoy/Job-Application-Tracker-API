using FluentValidation;
using JobApplicationTracker.Application.Common.Exceptions;
using JobApplicationTracker.Application.Common.Models;

namespace JobApplicationTracker.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail("The requested resource was not found."));
            }
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(
                exception.Errors.Select(x => new
                {
                    x.PropertyName,
                    x.ErrorMessage
                })));
        }
        catch (BadRequestException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(exception.Message));
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(exception.Message));
        }
        catch (UnauthorizedException exception)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("An unexpected server error occurred."));
        }
    }
}
