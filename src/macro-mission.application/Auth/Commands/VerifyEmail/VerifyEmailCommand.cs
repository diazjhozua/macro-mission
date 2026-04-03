using ErrorOr;
using MediatR;

namespace MacroMission.Application.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Token) : IRequest<ErrorOr<Success>>;
