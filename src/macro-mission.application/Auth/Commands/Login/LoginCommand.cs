using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Messaging;

namespace MacroMission.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<AuthResult>;
