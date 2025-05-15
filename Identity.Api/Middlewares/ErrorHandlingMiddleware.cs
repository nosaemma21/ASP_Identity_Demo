using System;
using System.Net;
using System.Text.Json;
using Identity.Api.CustomErrors;

namespace Identity.Api.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (JwtException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { message = "A Jwt error occured", err = ex.Message };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new { message = "An error occured", err = ex.Message };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
