using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicServices.Models;
using RestSharp;
using Services.Infrastructure;
using Synchronizer.Core.Helpers;

namespace Synchronizer.Core.VK;

public class VkMusicClient : IVkMusicClient
{
    private const string VkBaseUrl = "vk/music";
    private static readonly string HostUrl = Environment.GetEnvironmentVariable("vkserviceUrl")!;
    private readonly ILogger<VkMusicClient> logger;

    public VkMusicClient(ILogger<VkMusicClient> logger)
    {
        this.logger = logger;
    }

    public async Task<Result<Playlist[]>> GetUsersOwnPlaylistsAsync(string username)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{VkBaseUrl}/own/playlists");
        request.AddQueryParameter("username", username);
        var response = await client.ExecuteGetAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result.Fail<Playlist[]>("Auth fail");
        }
        var playlists = JsonSerializer.Deserialize<Playlist[]>(response.Content!) ?? Array.Empty<Playlist>();
        return playlists.AsResult();
    }

    public async Task<Result<None>> AddLinkedAccountAsync(string username, string login, string password, string? code)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{VkBaseUrl}/auth");
        request.AddBody(new LoginModel
            { Username = username, Login = login, Password = password, SecondFactorCode = code });
        try
        {
            await client.PostAsync(request);
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
        var request = new RestRequest($"{VkBaseUrl}/account/info");
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
}