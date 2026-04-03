using MacroMission.Domain.Users;

namespace MacroMission.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();

    // Returns the hashed form used for storage — raw token goes to the client.
    string HashRefreshToken(string rawToken);
}
