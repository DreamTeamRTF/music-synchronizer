using System.Text.Json.Serialization;

namespace MusicServices.Models;

public class SmartPlaylistUpdateModel
{
    public SmartPlaylistUpdateModel(string username, long playlistId, string[]? tracksToAdd, string[]? tracksToRemove)
    {
        TracksToAdd = tracksToAdd;
        TracksToRemove = tracksToRemove;
        Username = username;
        PlaylistId = playlistId;
    }

    [JsonPropertyName("playlistId")] public long PlaylistId { get; private set; }
    [JsonPropertyName("username")] public string Username { get; private set; }
    [JsonPropertyName("tracksToAdd")] public string[]? TracksToAdd { get; private set; }
    [JsonPropertyName("tracksToRemove")] public string[]? TracksToRemove { get; private set; }
}