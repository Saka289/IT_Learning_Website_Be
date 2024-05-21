using LW.Contracts.Domains;
using LW.Data.Common.Interfaces;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LW.Data.Common;

public class RepositoryBaseAsync<T, K> : RepositoryQueryBase<T, K>, IRepositoryBase<T, K> where T : EntityBase<K>
{
    private readonly AppDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryBaseAsync(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentOutOfRangeException(nameof(dbContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentOutOfRangeException(nameof(unitOfWork));
    }

    public void Create(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public async Task<K> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await SaveChangesAsync();
        return entity.Id;
    }

    public IList<K> CreateList(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().AddRange(entities);
        return entities.Select(x => x.Id).ToList();
    }

    public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        await SaveChangesAsync();
        return entities.Select(x => x.Id).ToList();
    }

    public void Update(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;

        T exits = _dbContext.Set<T>().Find(entity.Id);
        _dbContext.Entry(exits).CurrentValues.SetValues(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;

        T exist = _dbContext.Set<T>().Find(entity.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);

        await SaveChangesAsync();
    }

    public void UpdateList(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().AddRange(entities);
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        await SaveChangesAsync();
    }

    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await SaveChangesAsync();
    }

    public void DeleteList(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }

    public async Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _unitOfWork.CommitAsync();
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _dbContext.Database.BeginTransactionAsync();
    }

    public async Task EndTransactionAsync()
    {
        await SaveChangesAsync();
        await _dbContext.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync()
    {
        return _dbContext.Database.RollbackTransactionAsync();
    }
}