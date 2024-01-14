using System.Text.Json.Serialization;

namespace MusicServices.Models;

public class LoginModel
{
    [JsonPropertyName("username")] public string Username { get; init; }
    [JsonPropertyName("login")] public string Login { get; init; }

    [JsonPropertyName("password")] public string Password { get; init; }
    [JsonPropertyName("code")] public string? SecondFactorCode { get; init; }
}