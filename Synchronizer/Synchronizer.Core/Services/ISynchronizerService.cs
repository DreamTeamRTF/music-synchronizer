using MusicServices.Models;
using Services.Infrastructure;
using Synchronizer.DAL.Entities;

namespace Synchronizer.Core.Services;

public interface ISynchronizerService
{
    public Task<Result<Playlist>> SyncPlaylistsAsync(string username, long id, MusicServiceType serviceType);
    public Task<Result<PlaylistWithServiceType[]>> GetSynchronizedPlaylistsAsync(string username);
}