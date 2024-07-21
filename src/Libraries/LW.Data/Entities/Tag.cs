using System.ComponentModel.DataAnnotations;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Tag: EntityAuditBase<int>
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? KeyWord { get; set; }
    [Required]
    public bool IsActive { get; set; }
}