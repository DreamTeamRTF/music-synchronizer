using System.Text.Json.Serialization;

namespace MusicServices.Models;

public class PlaylistToAddRequest
{
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("playlist")] public Playlist Playlist { get; set; }
}