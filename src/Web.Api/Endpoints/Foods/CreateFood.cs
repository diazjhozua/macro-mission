using System.Security.Claims;
using MacroMission.Api.Extensions;
using MacroMission.Api.Infrastructure;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.CreateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Contracts.Foods;
using MacroMission.Domain.Common;

namespace MacroMission.Api.Endpoints.Foods;

internal sealed class CreateFood : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/foods", async (
            CreateFoodRequest request,
            ClaimsPrincipal user,
            ICommandHandler<CreateFoodCommand, FoodResult> handler,
            CancellationToken cancellationToken) =>
        {
            CreateFoodCommand command = new(
                user.GetUserId(),
                request.Name,
                request.Brand,
                request.Calories,
                request.Protein,
                request.Carbs,
                request.Fat,
                request.Fiber);

            Result<FoodResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                food => Results.Created($"api/v1/foods/{food.Id}", food),
                CustomResults.Problem);
        })
        .WithTags(Tags.Foods)
        .WithSummary("Create a custom food scoped to your account.")
        .Produces<FoodResult>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}
