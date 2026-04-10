using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetUserById;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetCurrentUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/users/me", async (
            ClaimsPrincipal user,
            IQueryHandler<GetUserByIdQuery, UserSummaryResult> handler,
            CancellationToken cancellationToken) =>
        {
            Result<UserSummaryResult> result = await handler.Handle(
                new GetUserByIdQuery(user.GetUserId()), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get the current authenticated user's profile.")
        .Produces<UserSummaryResult>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
