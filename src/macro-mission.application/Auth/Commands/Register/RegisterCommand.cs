using ErrorOr;
using MediatR;

namespace MacroMission.Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Nickname) : IRequest<ErrorOr<Success>>;
