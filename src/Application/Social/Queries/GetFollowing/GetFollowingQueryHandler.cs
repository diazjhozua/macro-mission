using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Queries.GetFollowers;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Users;
using MongoDB.Bson;

namespace MacroMission.Application.Social.Queries.GetFollowing;

internal sealed class GetFollowingQueryHandler(
    IFollowRepository followRepository,
    IUserRepository userRepository) : IQueryHandler<GetFollowingQuery, List<UserSummaryResult>>
{
    public async Task<Result<List<UserSummaryResult>>> Handle(
        GetFollowingQuery query,
        CancellationToken cancellationToken)
    {
        List<ObjectId> followingIds = await followRepository
            .GetFollowingIdsAsync(query.UserId, cancellationToken);

        if (followingIds.Count == 0)
            return Result<List<UserSummaryResult>>.Success([]);

        List<User> users = await userRepository.GetByIdsAsync(followingIds, cancellationToken);

        return Result<List<UserSummaryResult>>.Success(
            users.Select(GetFollowersQueryHandler.ToSummary).ToList());
    }
}
