using MacroMission.Api.Middleware;
using MacroMission.Application.DependencyInjection;
using MacroMission.Infrastructure.DependencyInjection;
using InfrastructureServiceExtensions = MacroMission.Infrastructure.DependencyInjection.InfrastructureServiceExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
