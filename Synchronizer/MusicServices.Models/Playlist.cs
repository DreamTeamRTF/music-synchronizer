using System.Text.Json.Serialization;

namespace MusicServices.Models;

/// <summary>
///     Модель плейлиста для всех сервисов будет расширяться
/// </summary>
public class Playlist
{
    public Playlist(long id, string title, string imageUrl, Track[]? tracks)
    {
        Title = title;
        ImageUrl = imageUrl;
        Tracks = tracks;
        Id = id;
    }

    [JsonPropertyName("id")] public long Id { get; private set; }

    [JsonPropertyName("title")] public string Title { get; private set; }

    [JsonPropertyName("imageUrl")] public string ImageUrl { get; private set; }

    [JsonPropertyName("tracks")] public Track[]? Tracks { get; private set; }
}