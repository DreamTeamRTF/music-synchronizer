using System.Net;
using System.Text.Json;
using MusicServices.Models;
using RestSharp;
using Services.Infrastructure;
using Synchronizer.Core;
using Synchronizer.Core.DTO;
using Synchronizer.Core.Helpers;
using Synchronizer.DAL.Entities;

namespace Synchronizer.WebApp.Services;

public class SynchronizerClient
{
    private const string SyncUrl = "sync";

    private static readonly string HostUrl =
        Environment.GetEnvironmentVariable("synchronizerServiceUrl") ?? "http://localhost";

    private readonly ILogger<SynchronizerClient> logger;

    public SynchronizerClient(ILogger<SynchronizerClient> logger)
    {
        this.logger = logger;
    }

    public async Task<Result<Playlist>> SyncPlaylist(string username, long playlistId, MusicServiceType serviceType)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{SyncUrl}/playlist");
        request.AddJsonBody(new PlaylistToSyncDto
            { Username = username, PlaylistId = playlistId, MusicService = serviceType });
        var response = await client.ExecutePostAsync(request);
        logger.LogInformation("Status code of sync: {ResponseStatusCode}, {Content}", response.StatusCode,
            response.Content);
        if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Fail<Playlist>("Auth fail");

        var model = JsonSerializer.Deserialize<Playlist>(response.Content!);
        return model ?? Result.Fail<Playlist>("Deserialization fail");
    }

    public async Task<Result<Playlist>> UpdatePlaylist(string username, long playlistId, MusicServiceType serviceType)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{SyncUrl}/playlists/update");
        request.AddJsonBody(new PlaylistToSyncDto
            { Username = username, PlaylistId = playlistId, MusicService = serviceType });
        var response = await client.ExecutePostAsync(request);
        logger.LogInformation("Status code of sync: {ResponseStatusCode}, {Content}", response.StatusCode,
            response.Content);
        if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Fail<Playlist>("Auth fail");

        var model = JsonSerializer.Deserialize<Playlist>(response.Content!);
        return model ?? Result.Fail<Playlist>("Deserialization fail");
    }

    public async Task<Result<PlaylistWithServiceType[]>> GetSynchronizedPlaylists(string username)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{SyncUrl}/playlists");
        request.AddQueryParameter("username", username);
        var response = await client.ExecuteGetAsync(request);
        logger.LogInformation("Status code of sync: {ResponseStatusCode}, {Content}", response.StatusCode,
            response.Content);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return Result.Fail<PlaylistWithServiceType[]>("Auth fail");

        var model = JsonSerializer.Deserialize<PlaylistWithServiceType[]>(response.Content!);
        return model ?? Result.Fail<PlaylistWithServiceType[]>("Deserialization fail");
    }
}