using Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ValidationException = Application.Exceptions.ValidationException;

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
            // Default values
            int statusCode = StatusCodes.Status500InternalServerError;
            string title = "An unexpected error occurred";
            object? response = null;

            switch (ex)
            {
                // 400 - Validation Exception (FluentValidation)
                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Validation Error";

                    // Create detailed validation error response
                    response = new
                    {
                        type = "https://httpstatuses.com/400",
                        title = title,
                        status = statusCode,
                        detail = "One or more validation errors occurred",
                        instance = context.Request.Path,
                        traceId = Activity.Current?.Id ?? context.TraceIdentifier,
                        errors = validationEx.Errors ?? new Dictionary<string, string[]>()
                    };
                    break;

                // 400 - Argument Exceptions
                case ArgumentException argEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Invalid Request";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/400",
                        Title = title,
                        Status = statusCode,
                        Detail = argEx.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // 401 - Unauthorized
                case AuthException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Authentication Required";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/401",
                        Title = title,
                        Status = statusCode,
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // 403 - Forbidden
                case ForbiddenException:
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Access Denied";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/403",
                        Title = title,
                        Status = statusCode,
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // 404 - Not Found
                case NotFoundException:
                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Resource Not Found";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/404",
                        Title = title,
                        Status = statusCode,
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // 409 - Conflict
                case ConflictException:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Resource Conflict";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/409",
                        Title = title,
                        Status = statusCode,
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // 409 - Concurrency
                case ConcurrencyException:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Concurrency Conflict";
                    response = new
                    {
                        type = "https://httpstatuses.com/409",
                        title = title,
                        status = statusCode,
                        detail = ex.Message,
                        instance = context.Request.Path,
                        traceId = Activity.Current?.Id ?? context.TraceIdentifier,
                        retryable = true
                    };
                    break;

                // 422 - Business Rule Violation
                case BusinessRuleException:
                    statusCode = StatusCodes.Status422UnprocessableEntity;
                    title = "Business Rule Violation";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/422",
                        Title = title,
                        Status = statusCode,
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };
                    break;

                // 500 - Server Errors
                case ServiceException:
                case ApplicationException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = "Service Error";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/500",
                        Title = title,
                        Status = statusCode,
                        Detail = "An error occurred while processing your request",
                        Instance = context.Request.Path
                    };
                    break;

                // Default - Unknown error
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = "An unexpected error occurred";
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/500",
                        Title = title,
                        Status = statusCode,
                        Detail = "An unexpected error occurred. Please try again later.",
                        Instance = context.Request.Path
                    };
                    break;
            }

            // Log the exception
            LogException(ex, statusCode);

            // Set response properties
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            // Add trace ID to response if not already present
            if (response is ProblemDetails problemDetails)
            {
                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            }

            // Serialize and write response
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private void LogException(Exception ex, int statusCode)
        {
            if (statusCode >= 500)
            {
                _logger.LogError(ex, "Server error: {Message} - {StackTrace}", ex.Message, ex.StackTrace);
            }
            else if (statusCode == 404)
            {
                _logger.LogInformation(ex, "Resource not found: {Message}", ex.Message);
            }
            else if (ex is ValidationException)
            {
                var validationEx = ex as ValidationException;
                var errors = validationEx?.Errors != null
                    ? string.Join(", ", validationEx.Errors.SelectMany(e => e.Value))
                    : ex.Message;
                _logger.LogWarning("Validation failed: {Errors}", errors);
            }
            else
            {
                _logger.LogWarning(ex, "Client error: {Message}", ex.Message);
            }
        }
    }
}