using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Post: EntityAuditBase<int>
{
    [Required]
    public string Content { get; set; }
    [Required]
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
    
    [Required]
    public int GradeId { get; set; }
    [ForeignKey(nameof(GradeId))]
    public virtual Grade Grade { get; set; }
    public ICollection<PostComment> PostComments { get; set; }
    public virtual ICollection<FavoritePost> FavoritePosts { get; set; }

}