using Application;
using Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IStringLocalizer<SharedResource> localizer)
        {
            _next = next;
            _logger = logger;
            _localizer = localizer;
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
            string title = _localizer["Error_Unexpected"];
            object? response = null;

            switch (ex)
            {
                // 400 - Validation Exception (FluentValidation)
                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = _localizer["Error_ValidationTitle"];

                    // Create detailed validation error response
                    response = new
                    {
                        type = "https://httpstatuses.com/400",
                        title = title,
                        status = statusCode,
                        detail = _localizer["Error_ValidationDetail"],
                        instance = context.Request.Path,
                        traceId = Activity.Current?.Id ?? context.TraceIdentifier,
                        errors = validationEx.Errors ?? new Dictionary<string, string[]>()
                    };
                    break;

                // 400 - Argument Exceptions
                case ArgumentException argEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = _localizer["Error_InvalidRequest"];
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
                    title = _localizer["Error_AuthRequired"];
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
                    title = _localizer["Error_AccessDenied"];
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
                    title = _localizer["Error_NotFound"];
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
                    title = _localizer["Error_Conflict"];
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
                    title = _localizer["Error_Concurrency"];
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
                    title = _localizer["Error_BusinessRule"];
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
                    title = _localizer["Error_Service"];
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/500",
                        Title = title,
                        Status = statusCode,
                        Detail = _localizer["Error_Processing"],
                        Instance = context.Request.Path
                    };
                    break;

                // Default - Unknown error
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = _localizer["Error_Unexpected"];
                    response = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/500",
                        Title = title,
                        Status = statusCode,
                        Detail = _localizer["Error_UnexpectedDetail"],
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