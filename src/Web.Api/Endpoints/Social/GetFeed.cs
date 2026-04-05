using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetFeed;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetFeed : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/feed", async (
            ClaimsPrincipal user,
            IQueryHandler<GetFeedQuery, List<PostResult>> handler,
            CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 20) =>
        {
            Result<List<PostResult>> result = await handler.Handle(
                new GetFeedQuery(user.GetUserId(), page, pageSize), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get paginated feed of posts from followed users.")
        .Produces<List<PostResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
