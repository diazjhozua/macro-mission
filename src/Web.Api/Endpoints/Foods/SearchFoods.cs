using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Queries.SearchFoods;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;


namespace MacroMission.Api.Endpoints.Foods;

internal sealed class SearchFoods : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/foods", async (
            ClaimsPrincipal user,
            IQueryHandler<SearchFoodsQuery, List<FoodResult>> handler,
            CancellationToken cancellationToken,
            string term = "",
            int page = 1,
            int pageSize = 20) =>
        {
            Result<List<FoodResult>> result = await handler.Handle(
                new SearchFoodsQuery(term, user.GetUserId(), page, pageSize),
                cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Foods)
        .WithSummary("Search global and your custom foods by name.")
        .Produces<List<FoodResult>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
