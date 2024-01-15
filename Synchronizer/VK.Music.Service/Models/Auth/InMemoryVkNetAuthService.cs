using System.Collections.Concurrent;
using VK.Music.Service.Configuration;
using VK.Music.Service.Exceptions;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VK.Music.Service.Models.Auth;

public class InMemoryVkNetAuthService : IVkNetApiAuthService
{
    private readonly ConcurrentDictionary<string, AuthorizationParameters> authorizedSessions = new();
    private readonly ITwoFactorVkProvider twoFactorProvider;
    private readonly VkApiFactory vkApiFactory;
    private readonly VkServiceConfig vkServiceConfig;

    public InMemoryVkNetAuthService(ITwoFactorVkProvider twoFactorProvider, VkServiceConfig vkServiceConfig,
        VkApiFactory vkApiFactory)
    {
        this.twoFactorProvider = twoFactorProvider;
        this.vkServiceConfig = vkServiceConfig;
        this.vkApiFactory = vkApiFactory;
    }

    public async Task CreateAuthSessionAsync(string username, string login, string password, string? code)
    {
        var vkNet = vkApiFactory.CreateApiClient();
        if (authorizedSessions.ContainsKey(username)) throw new ArgumentException();
        if (code != null) TwoFactorRequiredUsers.RequiredSecondFactor.TryAdd(username, code);

        await vkNet.AuthorizeAsync(new ApiAuthParams
        {
            Login = login,
            Password = password,
            ApplicationId = vkServiceConfig.ApplicationId,
            Settings = Settings.Audio,
            TwoFactorAuthorization = () => twoFactorProvider.GetAuthCode(username),
            TwoFactorSupported = true
        });

        authorizedSessions.TryAdd(username,
            new AuthorizationParameters { Token = vkNet.Token, UserId = vkNet.UserId!.Value });
    }

    public async Task<VkApi> AuthAsync(VkApi api, string username)
    {
        if (authorizedSessions.TryGetValue(username, out var authParams))
        {
            try
            {
                await api.AuthorizeAsync(new ApiAuthParams
                {
                    AccessToken = authParams.Token,
                    UserId = authParams.UserId,
                    ApplicationId = vkServiceConfig.ApplicationId
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Сессия протухла для пользователя {authParams.UserId} with exception {e}");
                authorizedSessions.TryRemove(username, out _);
            }

            if (api.IsAuthorized) return api;
        }

        throw new AuthApiException("Вы не связали vk музыку");
    }

    public void AddTestToken(string username, AuthorizationParameters authParams)
    {
        authorizedSessions.TryAdd(username, authParams);
    }
}