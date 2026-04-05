using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Meals.Commands.DeleteMeal;

public sealed record DeleteMealCommand(ObjectId MealId, ObjectId UserId) : ICommand;
