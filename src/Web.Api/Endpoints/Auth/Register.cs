using MacroMission.Api.Infrastructure;
using MacroMission.Application.Auth.Commands.Register;
using MacroMission.Application.Common.Messaging;
using MacroMission.Contracts.Auth;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Auth;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/auth/register", async (
            RegisterRequest request,
            ICommandHandler<RegisterCommand> handler,
            CancellationToken cancellationToken) =>
        {
            RegisterCommand command = new(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.Nickname);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.Ok(new { message = "Registration successful. Please check your email to verify your account." }),
                CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithSummary("Register a new account. A verification email will be sent.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
