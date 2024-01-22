using System.Text.Json.Serialization;

namespace MusicServices.Models;

public class PlaylistUpdateModel
{
    public PlaylistUpdateModel(string username, long playlistId, Playlist source)
    {
        Username = username;
        PlaylistId = playlistId;
        Source = source;
    }

    [JsonPropertyName("playlistId")] public long PlaylistId { get; private set; }
    [JsonPropertyName("username")] public string Username { get; private set; }
    [JsonPropertyName("source")] public Playlist Source { get; private set; }
}