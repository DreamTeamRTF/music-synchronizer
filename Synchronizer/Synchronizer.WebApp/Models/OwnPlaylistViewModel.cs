using MusicServices.Models;

namespace Synchronizer.WebApp.Models;

public class OwnPlaylistViewModel
{
    public Playlist Playlist { get; set; }
    public bool IsSynchronized { get; set; }
}