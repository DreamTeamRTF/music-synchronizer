using System.Text.Json.Serialization;

namespace VK.Music.Service.Models.Auth;

public class LoginModel
{
    [JsonPropertyName("login")] public string Login { get; init; }

    [JsonPropertyName("password")] public string Password { get; init; }
}