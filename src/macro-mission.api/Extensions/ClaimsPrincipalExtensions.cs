using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MongoDB.Bson;

namespace MacroMission.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Extracts the userId from the JWT sub claim.</summary>
    public static ObjectId GetUserId(this ClaimsPrincipal user)
    {
        string? sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(sub) || !ObjectId.TryParse(sub, out ObjectId userId))
            throw new InvalidOperationException("User ID claim is missing or invalid.");

        return userId;
    }
}
