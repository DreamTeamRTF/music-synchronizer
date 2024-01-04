using System.Collections.Concurrent;
using VK.Music.Service.Configuration;
using VK.Music.Service.Exceptions;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VK.Music.Service.Models.Auth;

public class VkNetAuthService : IVkNetApiAuthService
{
    private readonly ConcurrentDictionary<string, AuthorizationParameters> authorizedSessions = new();
    private readonly ITwoFactorVkProvider twoFactorProvider;
    private readonly VkApiFactory vkApiFactory;
    private readonly VkServiceConfig vkServiceConfig;

    public VkNetAuthService(ITwoFactorVkProvider twoFactorProvider, VkServiceConfig vkServiceConfig,
        VkApiFactory vkApiFactory)
    {
        this.twoFactorProvider = twoFactorProvider;
        this.vkServiceConfig = vkServiceConfig;
        this.vkApiFactory = vkApiFactory;
    }

    public async Task CreateAuthSessionAsync(string login, string password)
    {
        var vkNet = vkApiFactory.CreateApiClient();
        if (authorizedSessions.ContainsKey(login)) throw new ArgumentException();

        await vkNet.AuthorizeAsync(new ApiAuthParams
        {
            Login = login,
            Password = password,
            ApplicationId = vkServiceConfig.ApplicationId,
            Settings = Settings.Audio,
            TwoFactorAuthorization = twoFactorProvider.GetAuthCode,
            TwoFactorSupported = true
        });

        authorizedSessions.TryAdd(login,
            new AuthorizationParameters { Token = vkNet.Token, UserId = vkNet.UserId!.Value });
    }

    public async Task<VkApi> AuthAsync(VkApi api, string login)
    {
        if (authorizedSessions.TryGetValue(login, out var authParams))
        {
            try
            {
                await api.AuthorizeAsync(new ApiAuthParams
                {
                    AccessToken = authParams.Token,
                    UserId = authParams.UserId
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Сессия протухла для пользователя {authParams.UserId} with exception {e}");
                authorizedSessions.Remove(login, out _);
            }

            if (api.IsAuthorized) return api;
        }

        throw new AuthApiException("Something went wrong in auth");
    }

    public void AddTestToken(string login, AuthorizationParameters authParams)
    {
        authorizedSessions.TryAdd(login, authParams);
    }
}