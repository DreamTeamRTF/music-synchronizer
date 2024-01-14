namespace Synchronizer.WebApp.Models.Synchronizer.Playlists;

public class SynchronizedPlaylistModel
{
    public string? Title { get; set; }
    public string? CoverImage { get; set; }
    public MusicServiceTypeModel? ServiceType { get; set; }
}