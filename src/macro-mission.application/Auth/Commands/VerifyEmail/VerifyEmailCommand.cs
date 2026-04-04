using MacroMission.Application.Common.Messaging;

namespace MacroMission.Application.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Token) : ICommand;
