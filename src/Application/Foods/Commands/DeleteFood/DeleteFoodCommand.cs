using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.Foods.Commands.DeleteFood;

public sealed record DeleteFoodCommand(ObjectId FoodId, ObjectId RequesterId) : ICommand;
