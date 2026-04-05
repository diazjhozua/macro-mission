using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Commands.AddComment;
using MacroMission.Application.Social.Commands.DeleteComment;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using MongoDB.Bson;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class AddDeleteCommentCommandHandlerTests
{
    private readonly IPostRepository _postRepository = Substitute.For<IPostRepository>();
    private readonly ICommentRepository _commentRepository = Substitute.For<ICommentRepository>();
    private readonly ObjectId _userId = ObjectId.GenerateNewId();
    private readonly Post _post = new() { AuthorId = ObjectId.GenerateNewId() };

    [Fact]
    public async Task AddComment_WhenPostExists_CreatesCommentAndIncrementsCount()
    {
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);

        AddCommentCommandHandler handler = new(_postRepository, _commentRepository);
        Result<CommentResult> result = await handler.Handle(
            new AddCommentCommand(_post.Id, _userId, "Great meal!"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Text.Should().Be("Great meal!");
        await _commentRepository.Received(1).CreateAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>());
        await _postRepository.Received(1).IncrementCommentsAsync(_post.Id, 1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddComment_WhenPostNotFound_ReturnsNotFoundError()
    {
        _postRepository.GetByIdAsync(Arg.Any<ObjectId>()).Returns((Post?)null);

        AddCommentCommandHandler handler = new(_postRepository, _commentRepository);
        Result<CommentResult> result = await handler.Handle(
            new AddCommentCommand(ObjectId.GenerateNewId(), _userId, "text"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteComment_WhenOwnComment_DeletesAndDecrementsCount()
    {
        Comment comment = new() { AuthorId = _userId, PostId = _post.Id };
        _commentRepository.GetByIdAsync(comment.Id).Returns(comment);

        DeleteCommentCommandHandler handler = new(_commentRepository, _postRepository);
        Result result = await handler.Handle(
            new DeleteCommentCommand(comment.Id, _userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _commentRepository.Received(1).DeleteAsync(comment.Id, Arg.Any<CancellationToken>());
        await _postRepository.Received(1).IncrementCommentsAsync(_post.Id, -1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteComment_WhenNotOwner_ReturnsForbiddenError()
    {
        Comment comment = new() { AuthorId = ObjectId.GenerateNewId(), PostId = _post.Id };
        _commentRepository.GetByIdAsync(comment.Id).Returns(comment);

        DeleteCommentCommandHandler handler = new(_commentRepository, _postRepository);
        Result result = await handler.Handle(
            new DeleteCommentCommand(comment.Id, _userId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
