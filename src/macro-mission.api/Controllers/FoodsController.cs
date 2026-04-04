using MacroMission.Api.Extensions;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.CreateFood;
using MacroMission.Application.Foods.Commands.DeleteFood;
using MacroMission.Application.Foods.Commands.UpdateFood;
using MacroMission.Application.Foods.Queries.GetFoodById;
using MacroMission.Application.Foods.Queries.SearchFoods;
using MacroMission.Application.Foods.Results;
using MacroMission.Contracts.Foods;
using MacroMission.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MacroMission.Api.Controllers;

[Authorize]
public sealed class FoodsController(
    ICommandHandler<CreateFoodCommand, FoodResult> createHandler,
    ICommandHandler<UpdateFoodCommand, FoodResult> updateHandler,
    ICommandHandler<DeleteFoodCommand> deleteHandler,
    IQueryHandler<SearchFoodsQuery, List<FoodResult>> searchHandler,
    IQueryHandler<GetFoodByIdQuery, FoodResult> getByIdHandler) : ApiController
{
    /// <summary>Search global and your custom foods by name.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FoodResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string term = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<List<FoodResult>> result = await searchHandler.Handle(
            new SearchFoodsQuery(term, User.GetUserId(), page, pageSize),
            cancellationToken);

        return result.Match(
            foods => Ok(foods.Select(MapToResponse)),
            Problem);
    }

    /// <summary>Get a food by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out ObjectId foodId))
            return BadRequest(new { message = "Invalid food ID format." });

        Result<FoodResult> result = await getByIdHandler.Handle(
            new GetFoodByIdQuery(foodId, User.GetUserId()),
            cancellationToken);

        return result.Match(
            food => Ok(MapToResponse(food)),
            Problem);
    }

    /// <summary>Create a custom food scoped to your account.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateFoodRequest request,
        CancellationToken cancellationToken)
    {
        CreateFoodCommand command = new(
            User.GetUserId(),
            request.Name,
            request.Brand,
            request.Calories,
            request.Protein,
            request.Carbs,
            request.Fat,
            request.Fiber);

        Result<FoodResult> result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            food => CreatedAtAction(nameof(GetById), new { id = food.Id }, MapToResponse(food)),
            Problem);
    }

    /// <summary>Update your custom food.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        string id,
        UpdateFoodRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out ObjectId foodId))
            return BadRequest(new { message = "Invalid food ID format." });

        UpdateFoodCommand command = new(
            foodId,
            User.GetUserId(),
            request.Name,
            request.Brand,
            request.Calories,
            request.Protein,
            request.Carbs,
            request.Fat,
            request.Fiber);

        Result<FoodResult> result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            food => Ok(MapToResponse(food)),
            Problem);
    }

    /// <summary>Delete your custom food.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out ObjectId foodId))
            return BadRequest(new { message = "Invalid food ID format." });

        Result result = await deleteHandler.Handle(
            new DeleteFoodCommand(foodId, User.GetUserId()),
            cancellationToken);

        return result.Match(
            () => NoContent(),
            Problem);
    }

    private static FoodResponse MapToResponse(FoodResult result) => new(
        result.Id,
        result.Name,
        result.Brand,
        result.IsCustom,
        result.Calories,
        result.Protein,
        result.Carbs,
        result.Fat,
        result.Fiber,
        result.CreatedAt,
        result.UpdatedAt);
}
