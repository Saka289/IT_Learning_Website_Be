using System.ComponentModel.DataAnnotations.Schema;
using LW.Contracts.Domains;

namespace LW.Data.Entities;

public class Topic :EntityAuditBase<int>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public int DocumentId { get; set; }
    [ForeignKey("DocumentId")]
    
    public virtual Document Document { get; set; }
    public ICollection<Material> Materials { get; set; }
}