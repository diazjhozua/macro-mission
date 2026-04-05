using FluentAssertions;
using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Social.Queries.GetExplorePosts;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;
using NSubstitute;

namespace MacroMission.Tests.Social;

public sealed class GetExplorePostsQueryHandlerTests
{
    private readonly IPostRepository _postRepository = Substitute.For<IPostRepository>();
    private readonly GetExplorePostsQueryHandler _handler;

    public GetExplorePostsQueryHandlerTests()
    {
        _handler = new GetExplorePostsQueryHandler(_postRepository);
    }

    [Fact]
    public async Task Handle_ReturnsPublicPosts()
    {
        // Arrange
        List<Post> posts =
        [
            new Post { Visibility = PostVisibility.Public, Caption = "Post 1" },
            new Post { Visibility = PostVisibility.Public, Caption = "Post 2" }
        ];
        _postRepository.GetPublicPostsAsync(1, 20).Returns(posts);

        // Act
        Result<List<PostResult>> result = await _handler.Handle(
            new GetExplorePostsQuery(1, 20), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WhenNoPublicPosts_ReturnsEmptyList()
    {
        // Arrange
        _postRepository.GetPublicPostsAsync(1, 20).Returns([]);

        // Act
        Result<List<PostResult>> result = await _handler.Handle(
            new GetExplorePostsQuery(1, 20), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_PassesPaginationToRepository()
    {
        // Arrange
        _postRepository.GetPublicPostsAsync(3, 10).Returns([]);

        // Act
        await _handler.Handle(new GetExplorePostsQuery(3, 10), CancellationToken.None);

        // Assert
        await _postRepository.Received(1).GetPublicPostsAsync(3, 10, Arg.Any<CancellationToken>());
    }
}
