using Microsoft.Extensions.Logging;
using MusicServices.Models;
using RestSharp;
using Services.Infrastructure;

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
    
    public Task<Playlist[]> GetUsersOwnPlaylistsAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<None>> AddLinkedAccountAsync(string username, string login, string password, string? code)
    {
        var client = new RestClient(HostUrl);
        var request = new RestRequest($"{YandexBaseUrl}/auth");
        request.AddBody(new LoginModel { Username = username, Login = login, Password = password, SecondFactorCode = code});
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
}