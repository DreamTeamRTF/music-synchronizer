using System.ComponentModel.DataAnnotations.Schema;

namespace Synchronizer.DAL.Entities;

public class Track : BaseEntity<long>
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Album { get; set; }
    public MusicServiceType MusicService { get; set; }
    public long PlaylistId { get; set; }

    [ForeignKey(nameof(PlaylistId))] public Playlist Playlist { get; set; }
}