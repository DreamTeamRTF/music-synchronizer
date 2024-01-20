using System.Text.Json.Serialization;
using MusicServices.Models;
using Synchronizer.DAL.Entities;

namespace Synchronizer.Core;

public class PlaylistWithServiceType
{
    [JsonPropertyName("playlist")] public Playlist Playlist { get; set; }
    [JsonPropertyName("serviceType")] public MusicServiceType ServiceType { get; set; }
}