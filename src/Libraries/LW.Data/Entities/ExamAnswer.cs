using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class ExamAnswer : EntityBase<int>
{
    public int NumberOfQuestion { get; set; }
    public char Answer { get; set; }
    public int ExamCodeId { get; set; }
    [ForeignKey(nameof(ExamCodeId))] 
    public virtual ExamCode? ExamCode { get; set; }
}