using ErrorOr;
using MacroMission.Application.Auth.Results;
using MediatR;

namespace MacroMission.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<AuthResult>>;
