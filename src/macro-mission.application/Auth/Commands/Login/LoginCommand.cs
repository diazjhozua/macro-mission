using ErrorOr;
using MacroMission.Application.Auth.Results;
using MediatR;

namespace MacroMission.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<ErrorOr<AuthResult>>;
