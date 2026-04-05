using MacroMission.Api.Infrastructure;
using MacroMission.Application.Auth.Commands.VerifyEmail;
using MacroMission.Application.Common.Messaging;
using MacroMission.Contracts.Auth;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Auth;

internal sealed class VerifyEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/auth/verify-email", async (
            VerifyEmailRequest request,
            ICommandHandler<VerifyEmailCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
                new VerifyEmailCommand(request.Token), cancellationToken);

            return result.Match(
                () => Results.Ok(new { message = "Email verified successfully." }),
                CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithSummary("Verify email address using the token sent during registration.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
