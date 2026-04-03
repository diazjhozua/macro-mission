using MacroMission.Application.Common.Interfaces;
using MacroMission.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MacroMission.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        MongoDbSettings settings = configuration
            .GetSection(MongoDbSettings.SectionName)
            .Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDB config section is missing.");

        // MongoClient is thread-safe and designed to be a singleton for the app lifetime.
        services.AddSingleton(settings);
        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        return services;
    }
}
