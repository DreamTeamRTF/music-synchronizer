using Microsoft.Extensions.Logging;
using MusicServices.Models;
using RestSharp;
using Services.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Synchronizer.Core.Yandex;

public class YandexMusicClient : IYandexMusicClient
{
    private const string YandexBaseUrl = "yandex/music";
    private static readonly string HostUrl = Environment.GetEnvironmentVariable("yandexserviceUrl")!;
    private readonly ILogger<YandexMusicClient> logger;

    public YandexMusicClient(ILogger<YandexMusicClient> logger)
    {
        this.logger = logger;
    }

    public async Task<Playlist[]> GetUsersOwnPlaylistsAsync(string username)
    {
        var client = new RestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}my/playlists");
        request.AddQueryParameter("username", username);
        var response = await client.GetAsync(request);
        return JsonSerializer.Deserialize<Playlist[]>(response.Content!) ?? Array.Empty<Playlist>();
    }

    public async Task<Result<None>> AddLinkedAccountAsync(string username, string login, string password, string? code)
    {
        var client = new RestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/auth");
        request.AddBody(new LoginModel
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
        var client = new RestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/account/info");
        request.AddQueryParameter("username", username);
        try
        {
            var response = await client.GetAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<AccountInfoModel>(response.Content!).AsResult()!
                :  Result.Fail<AccountInfoModel>(response.ErrorMessage);
        }
        catch(HttpRequestException e)
        {
            return Result.Fail<AccountInfoModel>(e.Message);
        }
    }
}