using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Meals.Results;
using MacroMission.Domain.Meals;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Commands.CreateMeal;

public sealed record CreateMealItemCommand(ObjectId FoodId, double Grams);

public sealed record CreateMealCommand(
    ObjectId UserId,
    MealType MealType,
    DateTime? Date,
    List<CreateMealItemCommand> Items) : ICommand<MealResult>;
