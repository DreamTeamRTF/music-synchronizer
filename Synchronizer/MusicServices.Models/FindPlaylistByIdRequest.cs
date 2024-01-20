namespace MusicServices.Models;

public class FindPlaylistByIdRequest
{
    public string Username { get; set; }
    public long PlaylistId { get; set; }
}