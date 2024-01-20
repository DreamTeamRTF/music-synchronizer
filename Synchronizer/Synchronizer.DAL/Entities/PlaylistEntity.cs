namespace Synchronizer.DAL.Entities;

public class PlaylistEntity : BaseEntity<long>
{
    public string Title { get; set; }
    public MusicServiceType MusicService { get; set; }
    public ICollection<Track> Tracks { get; } = new List<Track>();
}