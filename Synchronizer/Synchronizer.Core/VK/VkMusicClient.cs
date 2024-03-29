﻿using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicServices.Models;
using RestSharp;
using Services.Infrastructure;
using Synchronizer.Core.Helpers;
using Synchronizer.Models.Contracts.VK;

namespace Synchronizer.Core.VK;

public class VkMusicClient : IVkMusicClient
{
    private const string VkBaseUrl = "vk/music";
    private static readonly string HostUrl = Environment.GetEnvironmentVariable("vkServiceUrl") ?? "http://localhost";
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
        if (response.StatusCode == HttpStatusCode.Unauthorized) return Result.Fail<Playlist[]>("Auth fail");
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

    public async Task<Result<Playlist>> TryAddPlaylistAsync(string username, Playlist playlist)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{VkBaseUrl}/add/playlist");
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
        var request = new RestRequest($"{VkBaseUrl}/playlist/findById");
        request.AddQueryParameter("username", username);
        request.AddQueryParameter("playlistId", id);
        try
        {
            var response = await client.GetAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>(response.ErrorMessage!);
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }

    public async Task<Result<Playlist>> SmartUpdatePlaylistAsync(SmartPlaylistUpdateModel updateModel)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{VkBaseUrl}/playlist/smart-update");
        request.AddJsonBody(updateModel);
        try
        {
            var response = await client.PostAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>(
                    $"Failed to SMART update playlist {updateModel.PlaylistId} for user {updateModel.Username}," +
                    $" STATUS CODE{response.StatusCode}, {response.Content}");
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }

    public async Task<Result<Playlist>> UpdatePlaylistAsync(PlaylistUpdateModel updateModel)
    {
        var client = RestClientFactory.CreateRestClient(HostUrl);
        var request = new RestRequest($"{VkBaseUrl}/playlist/update");
        request.AddJsonBody(updateModel);
        try
        {
            var response = await client.PostAsync(request);
            return response.StatusCode == HttpStatusCode.OK
                ? JsonSerializer.Deserialize<Playlist>(response.Content!).AsResult()!
                : Result.Fail<Playlist>(
                    $"Failed to DEFAULT update playlist {updateModel.PlaylistId} for user {updateModel.Username}," +
                    $" STATUS CODE{response.StatusCode}, {response.Content}");
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<Playlist>(e.Message);
        }
    }
}