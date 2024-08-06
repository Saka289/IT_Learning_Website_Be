using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class ExecuteCode : EntityAuditBase<int>
{
    public string MainCode { get; set; }
    public string SampleCode { get; set; }
    public int ProblemId { get; set; }
    public int LanguageId { get; set; }
    [ForeignKey(nameof(ProblemId))]
    public virtual Problem Problem { get; set; }
    [ForeignKey(nameof(LanguageId))]
    public virtual ProgramLanguage ProgramLanguage { get; set; }
}