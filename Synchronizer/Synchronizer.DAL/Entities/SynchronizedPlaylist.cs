namespace Synchronizer.DAL.Entities;

public class SynchronizedPlaylistLink : BaseEntity<Guid>
{
    public string Username { get; set; }
    public long YandexPlaylistId { get; set; }
    public long VkPlaylistId { get; set; }
    public MusicServiceType MainMusicService { get; set; }
    public List<SyncTrack> Tracks { get; set; } = new();
}