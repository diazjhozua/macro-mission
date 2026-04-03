using ErrorOr;
using MacroMission.Application.Auth.Commands.Login;
using MacroMission.Application.Auth.Commands.RefreshToken;
using MacroMission.Application.Auth.Commands.Register;
using MacroMission.Application.Auth.Commands.VerifyEmail;
using MacroMission.Application.Auth.Results;
using MacroMission.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MacroMission.Api.Controllers;

public sealed class AuthController(ISender mediator) : ApiController
{
    /// <summary>Register a new account. A verification email will be sent.</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        RegisterCommand command = new(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Nickname);

        ErrorOr<Success> result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(new { message = "Registration successful. Please check your email to verify your account." }),
            Problem);
    }

    /// <summary>Verify email address using the token sent during registration.</summary>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailRequest request,
        CancellationToken cancellationToken)
    {
        VerifyEmailCommand command = new(request.Token);

        ErrorOr<Success> result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(new { message = "Email verified successfully." }),
            Problem);
    }

    /// <summary>Login and receive an access token and refresh token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        LoginCommand command = new(request.Email, request.Password);

        ErrorOr<AuthResult> result = await mediator.Send(command, cancellationToken);

        return result.Match(
            authResult => Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    /// <summary>Exchange a valid refresh token for a new access token and rotated refresh token.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        RefreshTokenCommand command = new(request.RefreshToken);

        ErrorOr<AuthResult> result = await mediator.Send(command, cancellationToken);

        return result.Match(
            authResult => Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    private static AuthResponse MapToAuthResponse(AuthResult result) =>
        new(result.AccessToken, result.RefreshToken, result.AccessTokenExpiresAt);
}
