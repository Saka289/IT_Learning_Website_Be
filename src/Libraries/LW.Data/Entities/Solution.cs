using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Solution : EntityAuditBase<int>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Coding { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
    public bool IsActive { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser ApplicationUser { get; set; }
    [ForeignKey(nameof(ProblemId))]
    public virtual Problem Problem { get; set; }
}