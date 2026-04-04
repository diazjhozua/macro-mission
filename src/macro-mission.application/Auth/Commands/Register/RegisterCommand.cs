using MacroMission.Application.Common.Messaging;

namespace MacroMission.Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Nickname) : ICommand;
