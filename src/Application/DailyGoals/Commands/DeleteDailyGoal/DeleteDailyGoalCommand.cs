using MacroMission.Application.Common.Messaging;
using MongoDB.Bson;

namespace MacroMission.Application.DailyGoals.Commands.DeleteDailyGoal;

public sealed record DeleteDailyGoalCommand(ObjectId GoalId, ObjectId UserId) : ICommand;
