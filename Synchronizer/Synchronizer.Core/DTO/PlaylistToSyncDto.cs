using Synchronizer.DAL.Entities;

namespace Synchronizer.Core.DTO;

public class PlaylistToSyncDto
{
    public long PlaylistId { get; set; }
    public string Username { get; set; }
    public MusicServiceType MusicService { get; set; }
}