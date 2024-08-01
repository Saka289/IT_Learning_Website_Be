using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class FavoritePost : EntityAuditBase<int>
{
    [Required]
    public string UserId { get; set; }
    [Required]
    public int PostId { get; set; }
    [ForeignKey(nameof(PostId))]
    public virtual Post Post { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
}