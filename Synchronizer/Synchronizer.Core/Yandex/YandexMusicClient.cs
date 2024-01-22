using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicServices.Models;
using RestSharp;
using Services.Infrastructure;
using Synchronizer.Core.Helpers;

namespace Synchronizer.Core.Yandex;

public class YandexMusicClient : IYandexMusicClient
{
    private const string YandexBaseUrl = "yandex/music";

    private static readonly string HostUrl =
        Environment.GetEnvironmentVariable("yandexServiceUrl") ?? "http://127.0.0.1";

    private readonly ILogger<YandexMusicClient> logger;

    public YandexMusicClient(ILogger<YandexMusicClient> logger)
    {
        this.logger = logger;
    }

    public async Task<Result<Playlist[]>> GetUsersOwnPlaylistsAsync(string username)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/my/playlists");
        request.AddQueryParameter("username", username);
        var response = await client.ExecuteGetAsync(request);
        logger.LogInformation("Got response status code: {StatusCode}, content{ResponseContent}", response.StatusCode,
            response.Content);
        if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Fail<Playlist[]>("Auth fail");
        var playlists = JsonSerializer.Deserialize<Playlist[]>(response.Content!) ?? Array.Empty<Playlist>();
        return playlists.AsResult();
    }

    public async Task<Result<None>> AddLinkedAccountAsync(string username, string login, string password, string? code)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/auth");
        request.AddJsonBody(new LoginModel
            { Username = username, Login = login, Password = password, SecondFactorCode = code });
        try
        {
            await client.PostAsync(request);
            logger.LogInformation("Auth for {Username} succeed", username);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail<None>(e.Message);
        }
    }

    public async Task<Result<AccountInfoModel>> GetAccountInfoAsync(string username)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/account/info");
        request.AddQueryParameter("username", username);
        try
        {
            var response = await client.GetAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<AccountInfoModel>(response.Content!).AsResult()!
                : Result.Fail<AccountInfoModel>(response.ErrorMessage!);
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<AccountInfoModel>(e.Message);
        }
    }

    public async Task<Result<Playlist>> TryAddPlaylistAsync(string username, Playlist playlist)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/add/playlist");
        request.AddJsonBody(new PlaylistToAddRequest { Username = username, Playlist = playlist });
        try
        {
            var response = await client.PostAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>(response.ErrorMessage!);
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }

    public async Task<Result<Playlist>> GetPlaylistByIdAsync(string username, long id)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/playlist/findById");
        request.AddQueryParameter("username", username);
        request.AddQueryParameter("playlistId", id);
        try
        {
            var response = await client.GetAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>($"Failed to find playlist {id} for user {username}");
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }

    public async Task<Result<Playlist>> SmartUpdatePlaylistAsync(SmartPlaylistUpdateModel updateModel)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/playlist/smart-update");
        request.AddJsonBody(updateModel);
        try
        {
            var response = await client.PostAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>(
                    $"Failed to update playlist {updateModel.PlaylistId} for user {updateModel.Username}");
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }

    public async Task<Result<Playlist>> UpdatePlaylistAsync(PlaylistUpdateModel updateModel)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/playlist/update");
        request.AddJsonBody(updateModel);
        try
        {
            var response = await client.PostAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>(
                    $"Failed to update playlist {updateModel.PlaylistId} for user {updateModel.Username}");
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }
}