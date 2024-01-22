using System.ComponentModel.DataAnnotations.Schema;

namespace Synchronizer.DAL.Entities;

public class SyncTrack : BaseEntity<long>
{
    public string Title { get; set; }
    public Guid LinkId { get; set; }

    [ForeignKey("LinkId")] public SynchronizedPlaylistLink Link { get; set; }
}