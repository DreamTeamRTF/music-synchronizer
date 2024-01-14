using MusicServices.Models;

namespace Synchronizer.Core;

public interface ISynchronizerService
{
    public Task<Playlist> SyncPlaylistAsync();
}