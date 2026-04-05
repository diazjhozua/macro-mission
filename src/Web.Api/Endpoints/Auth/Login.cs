using MacroMission.Api.Infrastructure;
using MacroMission.Application.Auth.Commands.Login;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Messaging;
using MacroMission.Contracts.Auth;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Auth;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/auth/login", async (
            LoginRequest request,
            ICommandHandler<LoginCommand, AuthResult> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AuthResult> result = await handler.Handle(
                new LoginCommand(request.Email, request.Password), cancellationToken);

            return result.Match(
                authResult => Results.Ok(new AuthResponse(
                    authResult.AccessToken,
                    authResult.RefreshToken,
                    authResult.AccessTokenExpiresAt)),
                CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithSummary("Login and receive an access token and refresh token.")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
