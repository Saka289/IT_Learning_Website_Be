using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class ProgramLanguage : EntityAuditBase<int>
{
    public string Name { get; set; }
    public int BaseId { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<ExecuteCode> ExecuteCodes { get; set; }
    public virtual ICollection<Submission> Submissions { get; set; }
}