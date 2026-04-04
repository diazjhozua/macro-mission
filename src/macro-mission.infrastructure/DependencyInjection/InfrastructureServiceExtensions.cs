using MacroMission.Application.Common.Interfaces;
using MacroMission.Infrastructure.Auth;
using MacroMission.Infrastructure.Persistence;
using MacroMission.Infrastructure.Persistence.Indexes;
using MacroMission.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MacroMission.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddMongo(services, configuration);
        AddAuth(services, configuration);
        AddEmail(services, configuration);

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        IMongoDbContext context = serviceProvider.GetRequiredService<IMongoDbContext>();
        await MongoIndexInitializer.InitializeAsync(context);
    }

    private static void AddMongo(IServiceCollection services, IConfiguration configuration)
    {
        MongoDbSettings settings = configuration
            .GetSection(MongoDbSettings.SectionName)
            .Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDB config section is missing.");

        // MongoClient is thread-safe and designed to be a singleton for the app lifetime.
        services.AddSingleton(settings);
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDailyGoalRepository, DailyGoalRepository>();
        services.AddScoped<IFoodRepository, FoodRepository>();
    }

    private static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt config section is missing.");

        services.AddSingleton(jwtSettings);
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }

    private static void AddEmail(IServiceCollection services, IConfiguration configuration)
    {
        SmtpSettings smtpSettings = configuration
            .GetSection(SmtpSettings.SectionName)
            .Get<SmtpSettings>()
            ?? throw new InvalidOperationException("Smtp config section is missing.");

        services.AddSingleton(smtpSettings);
        services.AddTransient<IEmailService, SmtpEmailService>();
    }
}
