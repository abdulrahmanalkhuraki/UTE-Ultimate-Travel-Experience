using Application.Interfaces;
using Application.Interfaces.Notifications;
using Application.Interfaces.User;
using Application.Interfaces.Auth;
using Application.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure.Data;
using Infrastructure.Email;
using Infrastructure.Notifications;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IAuthService, AuthService>();

        // Firebase Cloud Messaging for real-time push notifications.
        services.Configure<FirebaseSettings>(configuration.GetSection(FirebaseSettings.SectionName));
        InitializeFirebase(configuration);
        services.AddScoped<IRealtimeNotifier, FirebaseNotifier>();

        return services;
    }

    /// <summary>
    /// Initializes the default <see cref="FirebaseApp"/> from a service-account JSON file
    /// if one is configured and present. Missing configuration is not fatal: push is simply
    /// skipped until credentials are provided.
    /// </summary>
    private static void InitializeFirebase(IConfiguration configuration)
    {
        if (FirebaseApp.DefaultInstance is not null)
            return;

        var credentialsPath = configuration[$"{FirebaseSettings.SectionName}:CredentialsPath"];
        if (string.IsNullOrWhiteSpace(credentialsPath) || !File.Exists(credentialsPath))
            return;

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });
    }
}
