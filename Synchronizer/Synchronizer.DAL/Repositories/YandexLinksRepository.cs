using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL.Repositories;

public class YandexLinksRepository : IRepository<YandexLink, Guid>
{
    private readonly SynchronizerDbContext synchronizerDbContext;

    public YandexLinksRepository(SynchronizerDbContext synchronizerDbContext)
    {
        this.synchronizerDbContext = synchronizerDbContext;
    }

    public IQueryable<YandexLink> Items => synchronizerDbContext.YandexLinks;
    public Guid Insert(YandexLink value)
    {
        var inserted = synchronizerDbContext.Add(value);
        synchronizerDbContext.SaveChanges();
        return inserted.Entity.Id;
    }

    public async Task<Guid> InsertAsync(YandexLink value)
    {
        var inserted = await synchronizerDbContext.AddAsync(value);
        await synchronizerDbContext.SaveChangesAsync();
        return inserted.Entity.Id;
    }
    
    public void Update(YandexLink link)
    {
        synchronizerDbContext.YandexLinks.Update(link);
        synchronizerDbContext.SaveChanges();
    }
}