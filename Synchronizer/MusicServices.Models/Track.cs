using System.Text.Json.Serialization;

namespace MusicServices.Models;

/// <summary>
///     Моедль трека для всех клиентов, мб будет расширяться
/// </summary>
public class Track
{
    public Track(string name, string author, string? album)
    {
        Name = name;
        Author = author;
        Album = album;
    }

    [JsonPropertyName("name")] public string Name { get; private set; }

    [JsonPropertyName("author")] public string Author { get; private set; }

    [JsonPropertyName("album")] public string? Album { get; private set; }
}