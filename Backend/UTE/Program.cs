using Application.Common;
using Application.Interfaces;
using Application.Interfaces.Flight;
using Application.Interfaces.Hotel;
using Application.Interfaces.Notifications;
using Application.Interfaces.TourCompany;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using Application.Mappings;
using Application.Services;
using Application.Validators.Hotel;
using Application.Validators.TourCompany;
using Application.Validators.User;
using Domain.Interfaces;
using Domain.Validators;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Json;
using UTE.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ADD INFRASTRUCTURE SERVICES
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration);

// ==========================================
// 2. CONFIGURE JWT AUTHENTICATION
// ==========================================
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

// ==========================================
// 3. ADD CONTROLLERS
// ==========================================
builder.Services.AddControllers();



// ==========================================
// 3.1. CONFIGURE FORM OPTIONS (MULTIPART UPLOADS)
// ==========================================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10 MB
    options.ValueLengthLimit = 10_000_000;
    options.MemoryBufferThreshold = 10_000_000;

    Console.WriteLine("FormOptions configured: MultipartBodyLengthLimit = " + options.MultipartBodyLengthLimit);
});



// ==========================================
// 4. CONFIGURE VALIDATION RESPONSE
// ==========================================
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


        foreach (var kv in context.ModelState)
        {
            foreach (var err in kv.Value.Errors)
            {
                Console.WriteLine($"KEY = {kv.Key}");
                Console.WriteLine($"ERROR = {err.ErrorMessage}");
                Console.WriteLine($"EXCEPTION = {err.Exception?.Message}");
            }
        }

        return new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

// ==========================================
// 5. ADD SWAGGER/OPENAPI
// ==========================================
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

// ==========================================
// 6. ADD CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// ==========================================
// 7. REGISTER APPLICATION SERVICES
// ==========================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Hotel
builder.Services.AddScoped<HotelCreateValidator>();
builder.Services.AddScoped<HotelUpdateValidator>();
builder.Services.AddScoped<IHotelService, HotelService>();

// Flight
builder.Services.AddScoped<FlightCreateValidator>();
builder.Services.AddScoped<FlightUpdateValidator>();
builder.Services.AddScoped<IFlightService, FlightService>();

// User
builder.Services.AddScoped<UpdateMeValidator>();
builder.Services.AddScoped<CompleteProfileValidator>();
builder.Services.AddScoped<CompleteCompanyProfileValidator>();
builder.Services.AddScoped<IUserService, UserService>();

// TourCompany
builder.Services.AddScoped<TourCompanyCreateValidator>();
builder.Services.AddScoped<TourCompanyUpdateValidator>();
builder.Services.AddScoped<ITourCompanyService, TourCompanyService>();

// TourPackage
builder.Services.AddScoped<TourPackageCreateValidator>();
builder.Services.AddScoped<TourPackageUpdateValidator>();
builder.Services.AddScoped<ITourPackageService, TourPackageService>();

// Notifications (IRealtimeNotifier/Firebase is registered in AddInfrastructure)
builder.Services.AddScoped<INotificationService, NotificationService>();






builder.Services.AddMemoryCache();
// ==========================================
// 8. ADD AUTOMAPPER
// ==========================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<HotelProfile>();
    cfg.AddProfile<FlightProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<TourCompanyProfile>();
    cfg.AddProfile<TourPackageProfile>();
    cfg.AddProfile<NotificationProfile>();
});

// ==========================================
// 9. BUILD THE APPLICATION
// ==========================================
var app = builder.Build();


// ==========================================
// 11. CONFIGURE DEVELOPMENT TOOLS
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
}

// ==========================================
// 12. CONFIGURE MIDDLEWARE PIPELINE (ORDER MATTERS!)
// ==========================================
app.UseHttpsRedirection();
app.UseStaticFiles();     // Serves files from wwwroot (e.g. /uploads/profiles/xxx.jpg)
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();  // Must be before Authorization
app.UseAuthorization();
app.MapControllers();
// ==========================================
// 13. RUN THE APPLICATION
// ==========================================
app.Run();