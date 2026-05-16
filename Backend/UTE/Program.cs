using Application.DTOs.Hotel.Request;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Flight;
using Application.Interfaces.Hotel;
using Application.Mappings;
using Application.Services;
using Application.Validators.Hotel;
using Domain.Interfaces;
using Domain.Validators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
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
// 4. CONFIGURE VALIDATION RESPONSE
// ==========================================
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value is { Errors.Count: > 0 })
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed.",
            Detail = "One or more fields are invalid. Please review the errors and try again.",
            Instance = context.HttpContext.Request.Path
        };

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
builder.Services.AddMemoryCache();
// ==========================================
// 8. ADD AUTOMAPPER
// ==========================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<HotelProfile>();
    cfg.AddProfile<FlightProfile>();
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
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();  // Must be before Authorization
app.UseAuthorization();
app.MapControllers();
// ==========================================
// 13. RUN THE APPLICATION
// ==========================================
app.Run();