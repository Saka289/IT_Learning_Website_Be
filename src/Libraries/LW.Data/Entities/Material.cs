using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Material : EntityAuditBase<int>
{
    [Required] 
    public string Title { get; set; }
    [Required] 
    public string KeyWord { get; set; }
    [Required] 
    public string Content { get; set; }
    [Required] 
    public string FilePath { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required] 
    public int LessonId { get; set; }
    [ForeignKey(nameof(LessonId))] 
    public virtual Lesson Lesson { get; set; }
}