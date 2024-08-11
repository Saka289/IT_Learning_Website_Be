using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class TestCase : EntityAuditBase<int>
{
    public string? Input { get; set; }
    public string Output { get; set; }
    public bool IsHidden { get; set; }
    public bool IsActive { get; set; }
    public int ProblemId { get; set; }
    [ForeignKey(nameof(ProblemId))]
    public virtual Problem Problem { get; set; }
}