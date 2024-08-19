using LW.Shared.DTOs.FavoritePost;

namespace LW.Shared.DTOs.Post;

public class PostDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    
    public IEnumerable<string> Roles { get; set; }

    public int GradeId { get; set; }
    public string GradeTitle { get; set; }
    public int NumberOfComment { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public ICollection<FavoritePostDto> FavoritePosts { get; set; }
}