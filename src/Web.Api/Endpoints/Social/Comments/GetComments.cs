using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetComments;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Api.Endpoints.Social;

internal sealed class GetComments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/posts/{id}/comments", async (
            string id,
            IQueryHandler<GetCommentsQuery, List<CommentResult>> handler,
            CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 20) =>
        {
            if (!ObjectId.TryParse(id, out ObjectId postId))
                return Results.BadRequest(new { message = "Invalid post ID format." });

            Result<List<CommentResult>> result = await handler.Handle(
                new GetCommentsQuery(postId, page, pageSize), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Social)
        .WithSummary("Get comments for a post.")
        .Produces<List<CommentResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
