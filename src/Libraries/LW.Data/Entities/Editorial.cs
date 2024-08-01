using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Editorial : EntityAuditBase<int>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string? PublicId { get; set; }
    public string? Image { get; set; }
    public int ProblemId { get; set; }
    public virtual Problem Problem { get; set; }
}