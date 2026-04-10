using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetFollowers;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;

namespace MacroMission.Application.Social.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository) : IQueryHandler<GetUserByIdQuery, UserSummaryResult>
{
    public async Task<Result<UserSummaryResult>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(query.UserId.ToString(), cancellationToken);

        if (user is null)
            return Result<UserSummaryResult>.Failure(
                Error.NotFound("User.NotFound", "User not found."));

        return Result<UserSummaryResult>.Success(GetFollowersQueryHandler.ToSummary(user));
    }
}
