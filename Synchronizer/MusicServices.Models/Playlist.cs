using System.Text.Json.Serialization;

namespace MusicServices.Models;
/// <summary>
/// Модель плейлиста для всех сервисов будет расширяться
/// </summary>
public class Playlist
{
    public Playlist(long id, string title, string author, Track[]? tracks)
    {
        Title = title;
        Author = author;
        Tracks = tracks;
        Id = id;
    }

    [JsonPropertyName("id")] public long Id { get; private set; }

    [JsonPropertyName("title")] public string Title { get; private set; }

    [JsonPropertyName("author")] public string Author { get; private set; }

    [JsonPropertyName("tracks")] public Track[]? Tracks { get; private set; }
}