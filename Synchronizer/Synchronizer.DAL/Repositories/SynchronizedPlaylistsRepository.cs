using Microsoft.EntityFrameworkCore;
using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL.Repositories;

public class SynchronizedPlaylistsRepository : IRepository<SynchronizedPlaylistLink, Guid>
{
    public IQueryable<SynchronizedPlaylistLink> Items => dbContext.Links;
    private readonly SynchronizerDbContext dbContext;

    public SynchronizedPlaylistsRepository(SynchronizerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public Guid Insert(SynchronizedPlaylistLink value)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> InsertAsync(SynchronizedPlaylistLink value)
    {
        var entity = await dbContext.Links.AddAsync(value);
        await dbContext.SaveChangesAsync();
        return entity.Entity.Id;
    }
    
    public SynchronizedPlaylistLink[] GetLinksForUser(string username)
    {
        var entity = dbContext.Links.Where(x => x.Username == username);
        return entity.ToArray();
    }
    
    public async Task SwitchMainMusicServiceAsync(Guid linkId, MusicServiceType serviceType)
    {
        var playlist = await Items.FirstOrDefaultAsync(x => x.Id == linkId);
        if (playlist != null)
        {
            playlist.MainMusicService = serviceType;
            await dbContext.SaveChangesAsync();
        }
    }
}