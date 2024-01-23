using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL.Repositories;

public class VkLinksRepository : IRepository<VkLink, Guid>
{
    private readonly SynchronizerDbContext synchronizerDbContext;

    public VkLinksRepository(SynchronizerDbContext synchronizerDbContext)
    {
        this.synchronizerDbContext = synchronizerDbContext;
    }

    public IQueryable<VkLink> Items => synchronizerDbContext.VkLinks;
    public Guid Insert(VkLink value)
    {
        var inserted = synchronizerDbContext.Add(value);
        synchronizerDbContext.SaveChanges();
        return inserted.Entity.Id;
    }

    public async Task<Guid> InsertAsync(VkLink value)
    {
        var inserted = await synchronizerDbContext.AddAsync(value);
        await synchronizerDbContext.SaveChangesAsync();
        return inserted.Entity.Id;
    }
    
    public void Update(VkLink link)
    {
        synchronizerDbContext.VkLinks.Update(link);
        synchronizerDbContext.SaveChanges();
    }
}