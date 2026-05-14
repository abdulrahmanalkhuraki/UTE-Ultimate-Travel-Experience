using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Application.Common;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
          ?? throw new InvalidOperationException("Jwt section missing in configuration.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var ms = context.ModelState;

        var bodyParamName = context.ActionDescriptor.Parameters
            .OfType<ControllerParameterDescriptor>()
            .FirstOrDefault(p =>
                p.BindingInfo?.BindingSource == BindingSource.Body
                || (p.BindingInfo?.BindingSource is null
                    && !p.ParameterType.IsValueType
                    && p.ParameterType != typeof(string)
                    && p.ParameterType != typeof(CancellationToken)))
            ?.Name;

        bool IsBodyLevelError(KeyValuePair<string, ModelStateEntry?> kvp)
        {
            if (kvp.Value is null || kvp.Value.Errors.Count == 0) return false;
            if (kvp.Key == "$" || string.IsNullOrEmpty(kvp.Key)) return true;
            if (!string.IsNullOrEmpty(bodyParamName) &&
                kvp.Key.Equals(bodyParamName, StringComparison.OrdinalIgnoreCase)) return true;
            if (kvp.Value.Errors.Any(e => e.Exception is JsonException)) return true;
            return false;
        }

        var hasBodyParseError = ms.Any(IsBodyLevelError);

        Dictionary<string, string[]> errors;
        string title;
        string detail;

        if (hasBodyParseError)
        {
            var bodyParam = context.ActionDescriptor.Parameters
                .OfType<ControllerParameterDescriptor>()
                .FirstOrDefault(p =>
                    !string.IsNullOrEmpty(bodyParamName)
                    && p.Name.Equals(bodyParamName, StringComparison.OrdinalIgnoreCase));

            var perFieldErrors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (bodyParam is not null)
            {
                try
                {
                    var emptyInstance = Activator.CreateInstance(bodyParam.ParameterType);
                    if (emptyInstance is not null)
                    {
                        var vc = new ValidationContext(emptyInstance);
                        var results = new List<ValidationResult>();
                        Validator.TryValidateObject(emptyInstance, vc, results, validateAllProperties: true);

                        foreach (var r in results)
                        {
                            foreach (var member in r.MemberNames.DefaultIfEmpty(string.Empty))
                            {
                                var key = string.IsNullOrEmpty(member) ? "body" : member;
                                if (!perFieldErrors.TryGetValue(key, out var list))
                                {
                                    list = new List<string>();
                                    perFieldErrors[key] = list;
                                }
                                if (!string.IsNullOrWhiteSpace(r.ErrorMessage))
                                    list.Add(r.ErrorMessage);
                            }
                        }
                    }
                }
                catch
                {
                    // ignore reflection failures; fall back to a generic body error
                }
            }

            perFieldErrors["body"] = new List<string>
            {
                "Request body is missing or has invalid JSON format. Send the data as a JSON object in the request body (not as URL params)."
            };

            errors = perFieldErrors.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
            title = "Invalid or missing request body.";
            detail = "The request body could not be read. Send the data as JSON inside the request Body (not in URL params).";
        }
        else
        {
            errors = ms
                .Where(kvp => kvp.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            title = "Validation failed.";
            detail = "One or more fields are invalid. Please review the errors and try again.";
        }

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title,
            Detail = detail,
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "UTE Tourism API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {your JWT token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };
    options.AddSecurityDefinition("Bearer", jwtScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        var (status, message) = ex switch
        {
            ConflictException c  => (StatusCodes.Status409Conflict, c.Message),
            NotFoundException n  => (StatusCodes.Status404NotFound, n.Message),
            ForbiddenException f => (StatusCodes.Status403Forbidden, f.Message),
            AuthException a      => (StatusCodes.Status401Unauthorized, a.Message),
            _                    => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = message,
            Instance = context.Request.Path
        };
        await context.Response.WriteAsJsonAsync(problem);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
