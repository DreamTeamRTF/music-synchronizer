﻿using MusicServices.Models;
using VkNet.Model;

namespace VK.Music.Service.Models;

public interface IVkNetApiClient
{
    public Task<Audio[]> GetTracksFromPlaylistAsync(string username, int playlistId);
    public Task<AudioPlaylist[]> GetOwnPlaylistsAsync(string username);
    public Task<AccountInfoModel> GetAccountInfoAsync(string username);
}