using System.Reflection;
using MacroMission.Api.Endpoints;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MacroMission.Api.Extensions;

internal static class EndpointExtensions
{
    internal static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        ServiceDescriptor[] endpoints = assembly
            .DefinedTypes
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        t.IsAssignableTo(typeof(IEndpoint)))
            .Select(t => ServiceDescriptor.Transient(typeof(IEndpoint), t))
            .ToArray();

        services.TryAddEnumerable(endpoints);
        return services;
    }

    internal static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
            endpoint.MapEndpoint(builder);

        return app;
    }
}
