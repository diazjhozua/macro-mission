using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Social.Commands.AddComment;
using MacroMission.Application.Social.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Social;

namespace MacroMission.Application.Social.Queries.GetComments;

internal sealed class GetCommentsQueryHandler(
    ICommentRepository commentRepository) : IQueryHandler<GetCommentsQuery, List<CommentResult>>
{
    public async Task<Result<List<CommentResult>>> Handle(
        GetCommentsQuery query,
        CancellationToken cancellationToken)
    {
        List<Comment> comments = await commentRepository.GetByPostAsync(
            query.PostId, query.Page, query.PageSize, cancellationToken);

        return Result<List<CommentResult>>.Success(
            comments.Select(AddCommentCommandHandler.ToResult).ToList());
    }
}
