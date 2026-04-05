using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Messaging;

namespace MacroMission.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResult>;
