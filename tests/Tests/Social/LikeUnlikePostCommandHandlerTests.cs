using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Commands.LikePost;
using MacroMission.Application.Social.Commands.UnlikePost;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class LikeUnlikePostCommandHandlerTests
{
    private readonly IPostRepository _postRepository = Substitute.For<IPostRepository>();
    private readonly ILikeRepository _likeRepository = Substitute.For<ILikeRepository>();
    private readonly ObjectId _userId = ObjectId.GenerateNewId();
    private readonly Post _post = new() { AuthorId = ObjectId.GenerateNewId() };

    [Fact]
    public async Task Like_WhenValid_CreatesLikeAndIncrementsCount()
    {
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);
        _likeRepository.GetAsync(_userId, _post.Id).Returns((Like?)null);

        LikePostCommandHandler handler = new(_postRepository, _likeRepository);
        Result result = await handler.Handle(new LikePostCommand(_userId, _post.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _likeRepository.Received(1).CreateAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
        await _postRepository.Received(1).IncrementLikesAsync(_post.Id, 1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Like_WhenAlreadyLiked_ReturnsConflictError()
    {
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);
        _likeRepository.GetAsync(_userId, _post.Id).Returns(new Like());

        LikePostCommandHandler handler = new(_postRepository, _likeRepository);
        Result result = await handler.Handle(new LikePostCommand(_userId, _post.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Unlike_WhenValid_DeletesLikeAndDecrementsCount()
    {
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);
        _likeRepository.GetAsync(_userId, _post.Id).Returns(new Like { UserId = _userId, PostId = _post.Id });

        UnlikePostCommandHandler handler = new(_postRepository, _likeRepository);
        Result result = await handler.Handle(new UnlikePostCommand(_userId, _post.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _likeRepository.Received(1).DeleteAsync(_userId, _post.Id, Arg.Any<CancellationToken>());
        await _postRepository.Received(1).IncrementLikesAsync(_post.Id, -1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unlike_WhenNotLiked_ReturnsNotFoundError()
    {
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);
        _likeRepository.GetAsync(_userId, _post.Id).Returns((Like?)null);

        UnlikePostCommandHandler handler = new(_postRepository, _likeRepository);
        Result result = await handler.Handle(new UnlikePostCommand(_userId, _post.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
