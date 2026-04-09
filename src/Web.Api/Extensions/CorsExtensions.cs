using MacroMission.Api.Cors;

namespace MacroMission.Api.Extensions;

internal static class CorsExtensions
{
    internal const string PolicyName = "AllowFrontend";

    internal static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        CorsSettings settings = configuration
            .GetSection(CorsSettings.SectionName)
            .Get<CorsSettings>()
            ?? throw new InvalidOperationException("Cors config section is missing.");

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy
                    .WithOrigins(settings.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
