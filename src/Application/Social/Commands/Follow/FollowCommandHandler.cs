using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using DomainFollow = MacroMission.Domain.Social.Follow;

namespace MacroMission.Application.Social.Commands.Follow;

internal sealed class FollowCommandHandler(
    IFollowRepository followRepository,
    IUserRepository userRepository) : ICommandHandler<FollowCommand>
{
    public async Task<Result> Handle(FollowCommand command, CancellationToken cancellationToken)
    {
        if (command.FollowerId == command.FollowingId)
            return Result.Failure(Error.Validation("Follow.SelfFollow", "You cannot follow yourself."));

        bool userExists = await userRepository.ExistsByIdAsync(command.FollowingId, cancellationToken);
        if (!userExists)
            return Result.Failure(Error.NotFound("Follow.UserNotFound", "User not found."));

        bool alreadyFollowing = await followRepository.IsFollowingAsync(
            command.FollowerId, command.FollowingId, cancellationToken);

        if (alreadyFollowing)
            return Result.Failure(Error.Conflict("Follow.AlreadyFollowing", "You are already following this user."));

        DomainFollow follow = new()
        {
            FollowerId = command.FollowerId,
            FollowingId = command.FollowingId
        };

        await followRepository.CreateAsync(follow, cancellationToken);

        return Result.Success();
    }
}
