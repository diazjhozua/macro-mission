using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetFollowers;

internal sealed class GetFollowersQueryHandler(
    IFollowRepository followRepository,
    IUserRepository userRepository) : IQueryHandler<GetFollowersQuery, List<UserSummaryResult>>
{
    public async Task<Result<List<UserSummaryResult>>> Handle(
        GetFollowersQuery query,
        CancellationToken cancellationToken)
    {
        List<ObjectId> followerIds = await followRepository
            .GetFollowerIdsAsync(query.UserId, cancellationToken);

        if (followerIds.Count == 0)
            return Result<List<UserSummaryResult>>.Success([]);

        List<User> users = await userRepository.GetByIdsAsync(followerIds, cancellationToken);

        return Result<List<UserSummaryResult>>.Success(
            users.Select(ToSummary).ToList());
    }

    internal static UserSummaryResult ToSummary(User user) => new(
        user.Id.ToString(),
        user.Nickname,
        user.FirstName,
        user.LastName);
}
