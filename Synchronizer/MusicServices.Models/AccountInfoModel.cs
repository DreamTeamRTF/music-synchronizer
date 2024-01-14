using System.Text.Json.Serialization;

namespace MusicServices.Models;

public class AccountInfoModel
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("imageUrl")] public string ImageUrl { get; set; }
}