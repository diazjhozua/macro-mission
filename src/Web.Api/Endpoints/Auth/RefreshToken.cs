using MacroMission.Api.Infrastructure;
using MacroMission.Application.Auth.Commands.RefreshToken;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Messaging;
using MacroMission.Contracts.Auth;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Auth;

internal sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/auth/refresh-token", async (
            RefreshTokenRequest request,
            ICommandHandler<RefreshTokenCommand, AuthResult> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AuthResult> result = await handler.Handle(
                new RefreshTokenCommand(request.RefreshToken), cancellationToken);

            return result.Match(
                authResult => Results.Ok(new AuthResponse(
                    authResult.AccessToken,
                    authResult.RefreshToken,
                    authResult.AccessTokenExpiresAt)),
                CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithSummary("Exchange a valid refresh token for a new access token and rotated refresh token.")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
