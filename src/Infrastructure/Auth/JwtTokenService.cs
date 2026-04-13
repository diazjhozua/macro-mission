using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Domain.Users;
using Microsoft.IdentityModel.Tokens;

namespace MacroMission.Infrastructure.Auth;

public sealed class JwtTokenService(JwtSettings settings) : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        // JWT includes userId + emailVerified to avoid extra DB round-trips on hot paths.
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("emailVerified", user.IsEmailVerified.ToString().ToLower()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(settings.Secret));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(64);
        // URL-safe Base64 avoids encoding issues when the token is stored in
        // cookies. Standard Base64 uses '+' and '/' which some cookie parsers
        // percent-encode, causing the hash to not match on the next request.
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public string HashRefreshToken(string rawToken)
    {
        // SHA-256 hash so the raw token never touches the database.
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(hash);
    }
}
