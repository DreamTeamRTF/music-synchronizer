namespace Synchronizer.DAL.Repositories;

public interface IRepository<T, TId>
{
    public IQueryable<T> Items { get; }
    public TId Insert(T value);
    Task<TId> InsertAsync(T value);
}