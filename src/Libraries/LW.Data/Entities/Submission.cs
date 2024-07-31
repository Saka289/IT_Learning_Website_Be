using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;
using LW.Shared.Enums;

namespace LW.Data.Entities;

public class Submission : EntityAuditBase<int>
{
    public string SourceCode { get; set; }
    public EStatusSubmission Status { get; set; }
    public decimal ExecutionTime { get; set; }
    public decimal MemoryUsage { get; set; }
    public int LanguageId { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
    [ForeignKey(nameof(LanguageId))]
    public virtual ProgramLanguage ProgramLanguage { get; set; }
    [ForeignKey(nameof(ProblemId))]
    public virtual Problem Problem { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
}