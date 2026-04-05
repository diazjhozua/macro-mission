using MacroMission.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MacroMission.Infrastructure.Auth;

/// <summary>
/// Delegates to ASP.NET Core Identity's PasswordHasher which uses PBKDF2-SHA256
/// with 100k iterations — no need to roll our own hashing scheme.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<string> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(string.Empty, password);

    public bool Verify(string password, string passwordHash)
    {
        PasswordVerificationResult result = _hasher.VerifyHashedPassword(
            string.Empty, passwordHash, password);

        return result != PasswordVerificationResult.Failed;
    }
}
