using System.Text.Json.Serialization;

namespace MusicServices.Models;

/// <summary>
///     Моедль трека для всех клиентов, мб будет расширяться
/// </summary>
public class Track
{
    public Track(string title, string author, string? album)
    {
        Title = title;
        Author = author;
        Album = album;
    }

    [JsonPropertyName("name")] public string Title { get; private set; }

    [JsonPropertyName("author")] public string Author { get; private set; }

    [JsonPropertyName("album")] public string? Album { get; private set; }
}