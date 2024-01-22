using Microsoft.EntityFrameworkCore;
using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL.Repositories;

public class SynchronizedPlaylistsRepository : IRepository<SynchronizedPlaylistLink, Guid>
{
    private readonly SynchronizerDbContext dbContext;

    public SynchronizedPlaylistsRepository(SynchronizerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<SyncTrack> Tracks => dbContext.SyncTracks;
    public IQueryable<SynchronizedPlaylistLink> Items => dbContext.Links;

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

    public async Task<Guid> InsertAsync(SynchronizedPlaylistLink value, IEnumerable<SyncTrack> syncTracks)
    {
        var entity = await dbContext.Links.AddAsync(value);
        await dbContext.SaveChangesAsync();
        foreach (var track in syncTracks)
        {
            await dbContext.SyncTracks.AddAsync(new SyncTrack { Title = track.Title, LinkId = entity.Entity.Id });
            await dbContext.SaveChangesAsync();
        }

        return entity.Entity.Id;
    }

    public SynchronizedPlaylistLink[] GetLinksForUser(string username)
    {
        var entity = dbContext.Links.Include(x => x.Tracks).Where(x => x.Username == username);
        return entity.ToArray();
    }

    public void Update(SynchronizedPlaylistLink link)
    {
        dbContext.Links.Update(link);
        dbContext.SaveChanges();
    }

    public void RemoveSyncTracks(SyncTrack[] track)
    {
        dbContext.SyncTracks.RemoveRange(track);
        dbContext.SaveChanges();
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