using MacroMission.Domain.Common;
using MongoDB.Bson;

namespace MacroMission.Domain.Social;

public sealed class Post : Entity
{
    public ObjectId AuthorId { get; init; }
    public ObjectId MealId { get; init; }
    public string? Caption { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
}
