using LW.Contracts.Domains.Interfaces;

namespace LW.Contracts.Domains;

public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    public TKey Id { get; set; }
}