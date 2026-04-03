using System.Text;
using MacroMission.Api.Middleware;
using MacroMission.Application.DependencyInjection;
using MacroMission.Infrastructure.Auth;
using MacroMission.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using InfrastructureServiceExtensions = MacroMission.Infrastructure.DependencyInjection.InfrastructureServiceExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT auth — reads settings directly here so the middleware has them before DI resolves.
JwtSettings jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt config section is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Create indexes before accepting traffic — idempotent, safe on every boot.
await InfrastructureServiceExtensions.InitializeDatabaseAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
