using Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace UTE.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Determine status code and message based on exception type
            var (statusCode, message) = ex switch
            {
                ConflictException c => (StatusCodes.Status409Conflict, c.Message),
                NotFoundException n => (StatusCodes.Status404NotFound, n.Message),
                ForbiddenException f => (StatusCodes.Status403Forbidden, f.Message),
                AuthException a => (StatusCodes.Status401Unauthorized, a.Message),
                InvalidOperationException i => (StatusCodes.Status400BadRequest, i.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            // Log the exception
            if (statusCode >= 500)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            }
            else
            {
                _logger.LogWarning(ex, "Handled exception: {Message}", ex.Message);
            }

            // Set response properties
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            // Create ProblemDetails object (matching your UseExceptionHandler format)
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = message,
                Instance = context.Request.Path,
                Detail = ex.Message // Optional: add more details
            };

            // Add trace ID for debugging (helpful for production)
            problem.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;

            // Serialize and write response
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsJsonAsync(problem, options);
        }
    }
}