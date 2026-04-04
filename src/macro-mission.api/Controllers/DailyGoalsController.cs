using ErrorOr;
using MacroMission.Api.Extensions;
using MacroMission.Application.DailyGoals.Commands.CreateDailyGoal;
using MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;
using MacroMission.Application.DailyGoals.Commands.UpdateDailyGoal;
using MacroMission.Application.DailyGoals.Queries.GetAllDailyGoals;
using MacroMission.Application.DailyGoals.Queries.GetDailyGoalById;
using MacroMission.Application.DailyGoals.Results;
using MacroMission.Contracts.DailyGoals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MacroMission.Api.Controllers;

[Authorize]
public sealed class DailyGoalsController(ISender mediator) : ApiController
{
    /// <summary>Create a new daily goal for the authenticated user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(DailyGoalResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateDailyGoalRequest request,
        CancellationToken cancellationToken)
    {
        CreateDailyGoalCommand command = new(
            User.GetUserId(),
            request.Name,
            request.Calories,
            request.Protein,
            request.Carbs,
            request.Fat,
            request.Fiber);

        ErrorOr<DailyGoalResult> result = await mediator.Send(command, cancellationToken);

        return result.Match(
            goal => CreatedAtAction(nameof(GetById), new { id = goal.Id }, MapToResponse(goal)),
            Problem);
    }

    /// <summary>Get all daily goals for the authenticated user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DailyGoalResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        GetAllDailyGoalsQuery query = new(User.GetUserId());

        ErrorOr<List<DailyGoalResult>> result = await mediator.Send(query, cancellationToken);

        return result.Match(
            goals => Ok(goals.Select(MapToResponse)),
            Problem);
    }

    /// <summary>Get a specific daily goal by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DailyGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out ObjectId goalId))
            return BadRequest(new { message = "Invalid goal ID format." });

        GetDailyGoalByIdQuery query = new(goalId, User.GetUserId());

        ErrorOr<DailyGoalResult> result = await mediator.Send(query, cancellationToken);

        return result.Match(
            goal => Ok(MapToResponse(goal)),
            Problem);
    }

    /// <summary>Update a daily goal. Set IsActive to true to make it the current active goal.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DailyGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        string id,
        UpdateDailyGoalRequest request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out ObjectId goalId))
            return BadRequest(new { message = "Invalid goal ID format." });

        UpdateDailyGoalCommand command = new(
            goalId,
            User.GetUserId(),
            request.Name,
            request.IsActive,
            request.Calories,
            request.Protein,
            request.Carbs,
            request.Fat,
            request.Fiber);

        ErrorOr<DailyGoalResult> result = await mediator.Send(command, cancellationToken);

        return result.Match(
            goal => Ok(MapToResponse(goal)),
            Problem);
    }

    /// <summary>Delete a daily goal by ID.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out ObjectId goalId))
            return BadRequest(new { message = "Invalid goal ID format." });

        DeleteDailyGoalCommand command = new(goalId, User.GetUserId());

        ErrorOr<Deleted> result = await mediator.Send(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    private static DailyGoalResponse MapToResponse(DailyGoalResult result) => new(
        result.Id,
        result.Name,
        result.IsActive,
        result.Calories,
        result.Protein,
        result.Carbs,
        result.Fat,
        result.Fiber,
        result.CreatedAt,
        result.UpdatedAt);
}
