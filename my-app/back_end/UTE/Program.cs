using Application.Interfaces;
using Application.Interfaces.Admin;
using Application.Interfaces.Booking;
using Application.Interfaces.Companion;
using Application.Interfaces.Favorite;
using Application.Interfaces.Notifications;
using Application.Interfaces.Rate;
using Application.Interfaces.Review;
using Application.Interfaces.SupportReply;
using Application.Interfaces.Ticket;
using Application.Interfaces.TourCompany;
using Application.Interfaces.TouristGuide;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using Application.Mappings;
using Application.Services;
using Application.Validators.Booking;
using Application.Validators.Companion;
using Application.Validators.Rate;
using Application.Validators.Review;
using Application.Validators.SupportReply;
using Application.Validators.Ticket;
using Application.Validators.TourCompany;
using Application.Validators.TouristGuide;
using Application.Validators.User;
using Domain.Interfaces;
using Domain.Validators;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Seed;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.Localization;
using UTE.Middleware;
using UTE.Security;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ADD INFRASTRUCTURE SERVICES
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCompletedProfile", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("IsProfileCompleted", "true");
    });
});

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResponseHandler>();

// ==========================================
// 3. ADD CONTROLLERS
// ==========================================
builder.Services.AddControllers();

// ==========================================
// 3.1. CONFIGURE FORM OPTIONS (MULTIPART UPLOADS)
// ==========================================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000;
    options.ValueLengthLimit = 10_000_000;
    options.MemoryBufferThreshold = 10_000_000;
    options.MultipartHeadersLengthLimit = 32_768;
    options.MultipartHeadersCountLimit = 32;
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
                catch { }
            }

            perFieldErrors["body"] = new List<string>
            {
                "Request body is missing or has invalid JSON format. Send the data as a JSON object in the request body."
            };

            errors = perFieldErrors.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
            title = "Invalid or missing request body.";
            detail = "The request body could not be read. Send the data as JSON inside the request Body.";
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
// 6. ADD CORS (مُحدث ليدعم React, Flutter, وجميع المنصات)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // React Frontend (Vite)
                "http://localhost:3000",  // React Frontend (Alternative)
                "https://localhost:7016"  // Self / Local requests
              )
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .SetIsOriginAllowed(_ => true) // مسموح لأي مصدر محلي أو تطبيقات الموبايل (Flutter) التي لا ترسل Origin تقليدي
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ==========================================
// 7. REGISTER APPLICATION SERVICES
// ==========================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<Application.Interfaces.Country.ICountryService, CountryService>();
builder.Services.AddScoped<Application.Interfaces.City.ICityService, CityService>();

builder.Services.AddScoped<UserUpdateValidator>();
builder.Services.AddScoped<CompleteProfileValidator>();
builder.Services.AddScoped<UpdateLocationValidator>();
builder.Services.AddScoped<ChangePasswordValidator>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<BookingCreateValidator>();
builder.Services.AddScoped<BookingUpdateValidator>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<TourCompanyCreateValidator>();
builder.Services.AddScoped<TourCompanyUpdateValidator>();
builder.Services.AddScoped<ITourCompanyService, TourCompanyService>();

builder.Services.AddScoped<TourPackageCreateValidator>();
builder.Services.AddScoped<TourPackageUpdateValidator>();
builder.Services.AddScoped<ITourPackageService, TourPackageService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddScoped<TouristGuideCreateValidator>();
builder.Services.AddScoped<TouristGuideUpdateValidator>();
builder.Services.AddScoped<ITouristGuideService, TouristGuideService>();

builder.Services.AddScoped<CompanionCreateValidator>();
builder.Services.AddScoped<CompanionUpdateValidator>();
builder.Services.AddScoped<ICompanionService, CompanionService>();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<RateCreateValidator>();
builder.Services.AddScoped<IRateService, RateService>();

builder.Services.AddScoped<ReviewCreateValidator>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<TicketCreateValidator>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddScoped<SupportReplyCreateValidator>();
builder.Services.AddScoped<ISupportReplyService, SupportReplyService>();

builder.Services.AddScoped<IFavoriteService, FavoriteService>();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<Application.Interfaces.Localization.ILanguageContext, LanguageContext>();
builder.Services.AddScoped<Application.Interfaces.Localization.ILocalizedMapper, LocalizedMapper>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// ==========================================
// 7.1. ADD HANGFIRE (BACKGROUND JOBS)
// ==========================================
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<BookingBackgroundJobs>();

// ==========================================
// 8. ADD AUTOMAPPER
// ==========================================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<BookingProfile>();
    cfg.AddProfile<PaymentProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<TourCompanyProfile>();
    cfg.AddProfile<TourPackageProfile>();
    cfg.AddProfile<TouristGuideProfile>();
    cfg.AddProfile<NotificationProfile>();
    cfg.AddProfile<CountryProfile>();
    cfg.AddProfile<CityProfile>();
    cfg.AddProfile<PersonProfile>();
    cfg.AddProfile<CompanionProfile>();
    cfg.AddProfile<RateProfile>();
    cfg.AddProfile<ReviewProfile>();
    cfg.AddProfile<TicketProfile>();
    cfg.AddProfile<SupportReplyProfile>();
    cfg.AddProfile<CompletedTourPackageProfile>();
    cfg.AddProfile<ActiveTourPackageProfile>();
    cfg.AddProfile<CancelledTourPackageProfile>();
    cfg.AddProfile<RejectedTourPackageProfile>();
});

// ==========================================
// 9. BUILD THE APPLICATION
// ==========================================
var app = builder.Build();

Application.Common.Constants.ExceptionMessages.Initialize(
    app.Services.GetRequiredService<Microsoft.Extensions.Localization.IStringLocalizer<Application.SharedResource>>());

// ==========================================
// 10. SEED DATABASE
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDbSeedService>();
    await seeder.SeedAsync();
}

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

// 1. CORS يجب أن يكون في البداية المطلقة لمعالجة طلبات الـ Preflight (OPTIONS)
app.UseCors();

// 2. إعادة توجيه الـ HTTPS
app.UseHttpsRedirection();

app.UseStaticFiles();

var supportedCultures = Domain.Common.LanguageCodes.SupportedTags
    .Select(tag => new CultureInfo(tag))
    .ToArray();

app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture(Domain.Common.LanguageCodes.Default);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new UTE.Localization.LanguageRequestCultureProvider());
});

app.UseMiddleware<GlobalExceptionMiddleware>();

// 3. المصادقة والتفويض
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================================
// 12.1. CONFIGURE HANGFIRE DASHBOARD & RECURRING JOBS
// ==========================================
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireDashboardAuthFilter()]
});

RecurringJob.AddOrUpdate<BookingBackgroundJobs>(
    "process-started-bookings",
    j => j.ProcessStartedBookingsAsync(CancellationToken.None),
    "*/5 * * * *");

RecurringJob.AddOrUpdate<BookingBackgroundJobs>(
    "process-completed-bookings",
    j => j.ProcessCompletedBookingsAsync(CancellationToken.None),
    "*/5 * * * *");

RecurringJob.AddOrUpdate<BookingBackgroundJobs>(
    "send-upcoming-reminders",
    j => j.SendUpcomingBookingRemindersAsync(CancellationToken.None),
    "0 8 * * *");

RecurringJob.AddOrUpdate<BookingBackgroundJobs>(
    "send-registration-deadline-reminders",
    j => j.SendRegistrationDeadlineRemindersAsync(CancellationToken.None),
    "0 8 * * *");

RecurringJob.AddOrUpdate<BookingBackgroundJobs>(
    "process-completed-packages",
    j => j.ProcessCompletedPackagesAsync(CancellationToken.None),
    "0 0 * * *");

// ==========================================
// 13. RUN THE APPLICATION
// ==========================================
app.Run();