using System.Linq.Expressions;
using LW.Contracts.Domains;
using Microsoft.EntityFrameworkCore.Storage;

namespace LW.Data.Common.Interfaces;

public interface IRepositoryQueryBase<T, K>
    where T : EntityBase<K>
{
    IQueryable<T> FindAll(bool trackChanges = false);
    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    Task<T?> GetByIdAsync(K id);
    Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);
}

public interface IRepositoryBase<T, K> : IRepositoryQueryBase<T, K>
    where T : EntityBase<K>
{
    void Create(T entity);
    Task<K> CreateAsync(T entity);
    IList<K> CreateList(IEnumerable<T> entities);
    Task<IList<K>> CreateListAsync(IEnumerable<T> entities);
    void Update(T entity);
    Task UpdateAsync(T entity);
    void UpdateList(IEnumerable<T> entities);
    Task UpdateListAsync(IEnumerable<T> entities);
    void Delete(T entity);
    Task DeleteAsync(T entity);
    void DeleteList(IEnumerable<T> entities);
    Task DeleteListAsync(IEnumerable<T> entities);
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();
}