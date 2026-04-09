using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using MacroMission.Api.Extensions;
using MacroMission.Api.Middleware;
using MacroMission.Api.OpenApi;
using MacroMission.Application.DependencyInjection;
using MacroMission.Infrastructure.Auth;
using MacroMission.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using InfrastructureServiceExtensions = MacroMission.Infrastructure.DependencyInjection.InfrastructureServiceExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddRateLimiting();
builder.Services.AddCorsPolicy(builder.Configuration);

// JWT auth — reads settings directly here so the middleware has them before DI resolves.
JwtSettings jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt config section is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Disable default claim mapping so "sub" stays as "sub", not ClaimTypes.NameIdentifier.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

// Serialize enums as strings globally so responses return "Breakfast" instead of 0.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

WebApplication app = builder.Build();

// Create indexes before accepting traffic — idempotent, safe on every boot.
await InfrastructureServiceExtensions.InitializeDatabaseAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Title = "Macro Mission");
}

app.UseRateLimiter();
app.UseCors(CorsExtensions.PolicyName);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.Run();
