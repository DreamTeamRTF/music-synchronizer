using MusicServices.Models;
using Services.Infrastructure;
using Synchronizer.DAL.Entities;

namespace Synchronizer.Core.Services;

public interface ISynchronizerService
{
    public Task<Result<Playlist>> SyncPlaylistsAsync(string username, long id, MusicServiceType serviceType);
    public Task<Result<PlaylistWithServiceType[]>> GetSynchronizedPlaylistsAsync(string username);

    public Task<Result<Playlist>> SmartUpdateSyncPlaylistAsync(string username, long id, MusicServiceType serviceType);

    public Task<Result<Playlist>>
        DefaultUpdateSyncPlaylistAsync(string username, long id, MusicServiceType serviceType);
}