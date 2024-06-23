using LW.Contracts.Domains.Interfaces;

namespace LW.Contracts.Domains;

public abstract class EntityAuditBase<T> : EntityBase<T>, IAuditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }

    public string CreatedBy { get; set; }

    public string? LastModifiedBy { get; set; }
}