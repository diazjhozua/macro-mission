using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Meals;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Commands.UpdateMeal;

public sealed record UpdateMealItemCommand(ObjectId FoodId, double Grams);

public sealed record UpdateMealCommand(
    ObjectId MealId,
    ObjectId UserId,
    MealType MealType,
    List<UpdateMealItemCommand> Items) : ICommand<MealResult>;
