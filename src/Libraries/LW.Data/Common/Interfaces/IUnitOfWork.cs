namespace LW.Data.Common.Interfaces;

public interface IUnitOfWork : IDisposable 
{
    Task<int> CommitAsync();
}