using System.Linq.Expressions;
using LW.Contracts.Domains;
using LW.Data.Common.Interfaces;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Common;

public class RepositoryQueryBase<T, K> : IRepositoryQueryBase<T, K> where T : EntityBase<K>
{
    private readonly AppDbContext _dbContext;

    public RepositoryQueryBase(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
    }

    public IQueryable<T> FindAll(bool trackChanges = false) =>
        !trackChanges ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }
    
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false) =>
        !trackChanges ? _dbContext.Set<T>().Where(expressions).AsNoTracking() : _dbContext.Set<T>().Where(expressions);

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindByCondition(expressions, trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public async Task<T?> GetByIdAsync(K id)
    {
        return await FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
    {
        return await FindByCondition(x => x.Id.Equals(id), trackChanges: false, includeProperties)
            .FirstOrDefaultAsync();
    }
}