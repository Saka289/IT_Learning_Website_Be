using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LW.Data.Entities;

public class ExamImage: EntityBase<int>
{
    [Required]
    public string FilePath { get; set; }
    [Required]
    public string publicId { get; set; }
    [Required]
    public int Index { get; set; }
    [Required]
    public int ExamId { get; set; }

    [ForeignKey(nameof(ExamId))] 
    public virtual Exam Exam { get; set; }
}