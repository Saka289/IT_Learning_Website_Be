using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class ExamAnswer: EntityBase<int>
{
    public int NumberOfQuestion { get; set; }
    
    public char Answer { get; set; }
    
    public decimal Score { get; set; }
    public int ExamId { get; set; }
    [ForeignKey(nameof(ExamId))] 
    public virtual Exam Exam { get; set; }
}