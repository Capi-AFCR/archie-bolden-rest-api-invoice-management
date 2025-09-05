using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace RestAPIInvoiceManagement.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        if (exception is KeyNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            message = exception.Message;
        }
        else if (exception is ValidationException validationEx)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage));
        }

        context.Response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(new { Message = message });
        return context.Response.WriteAsync(result);
    }
}