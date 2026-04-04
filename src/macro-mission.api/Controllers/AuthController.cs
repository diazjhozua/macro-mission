using MacroMission.Application.Auth.Commands.Login;
using MacroMission.Application.Auth.Commands.RefreshToken;
using MacroMission.Application.Auth.Commands.Register;
using MacroMission.Application.Auth.Commands.VerifyEmail;
using MacroMission.Application.Auth.Results;
using MacroMission.Application.Common.Messaging;
using MacroMission.Contracts.Auth;
using MacroMission.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace MacroMission.Api.Controllers;

public sealed class AuthController(
    ICommandHandler<RegisterCommand> registerHandler,
    ICommandHandler<VerifyEmailCommand> verifyEmailHandler,
    ICommandHandler<LoginCommand, AuthResult> loginHandler,
    ICommandHandler<RefreshTokenCommand, AuthResult> refreshTokenHandler) : ApiController
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

        Result result = await registerHandler.Handle(command, cancellationToken);

        return result.Match(
            () => Ok(new { message = "Registration successful. Please check your email to verify your account." }),
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
        Result result = await verifyEmailHandler.Handle(
            new VerifyEmailCommand(request.Token), cancellationToken);

        return result.Match(
            () => Ok(new { message = "Email verified successfully." }),
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
        Result<AuthResult> result = await loginHandler.Handle(
            new LoginCommand(request.Email, request.Password), cancellationToken);

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
        Result<AuthResult> result = await refreshTokenHandler.Handle(
            new RefreshTokenCommand(request.RefreshToken), cancellationToken);

        return result.Match(
            authResult => Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    private static AuthResponse MapToAuthResponse(AuthResult result) =>
        new(result.AccessToken, result.RefreshToken, result.AccessTokenExpiresAt);
}
