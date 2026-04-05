using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetExplorePosts;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class Explore : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/explore", async (
            IQueryHandler<GetExplorePostsQuery, List<PostResult>> handler,
            CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 20) =>
        {
            Result<List<PostResult>> result = await handler.Handle(
                new GetExplorePostsQuery(page, pageSize), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Discover all public posts from everyone, sorted by newest first.")
        .Produces<List<PostResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
