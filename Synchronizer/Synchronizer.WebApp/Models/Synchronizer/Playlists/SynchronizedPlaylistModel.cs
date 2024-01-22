namespace Synchronizer.WebApp.Models.Synchronizer.Playlists;

public class SynchronizedPlaylistModel
{
    public long PlaylistId { get; set; }
    public string? Title { get; set; }
    public string? CoverImage { get; set; }
    public MusicServiceTypeModel? ServiceType { get; set; }
}