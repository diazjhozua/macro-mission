using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;

namespace MacroMission.Application.Social.Commands.Unfollow;

internal sealed class UnfollowCommandHandler(
    IFollowRepository followRepository) : ICommandHandler<UnfollowCommand>
{
    public async Task<Result> Handle(UnfollowCommand command, CancellationToken cancellationToken)
    {
        bool isFollowing = await followRepository.IsFollowingAsync(
            command.FollowerId, command.FollowingId, cancellationToken);

        if (!isFollowing)
            return Result.Failure(Error.NotFound("Follow.NotFollowing", "You are not following this user."));

        await followRepository.DeleteAsync(command.FollowerId, command.FollowingId, cancellationToken);

        return Result.Success();
    }
}
