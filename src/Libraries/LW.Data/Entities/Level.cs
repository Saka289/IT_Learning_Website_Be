using System.ComponentModel.DataAnnotations;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Level : EntityBase<int>
{
    [Required] public string Name { get; set; }
    
    public ICollection<Grade> Grades { get; set; }
    public ICollection<Exam> Exams { get; set; }
}