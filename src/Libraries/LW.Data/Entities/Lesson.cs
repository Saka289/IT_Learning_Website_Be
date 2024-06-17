using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Lesson : EntityAuditBase<int>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string KeyWord { get; set; }
    [Required] 
    public bool IsActive { get; set; }
    [Required]
    public int TopicId { get; set; }
    [ForeignKey(nameof(TopicId))]
    public virtual Topic Topic { get; set; }
    public virtual ICollection<Material> Materials { get; set; }
}