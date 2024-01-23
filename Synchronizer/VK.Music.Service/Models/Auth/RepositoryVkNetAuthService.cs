using Synchronizer.DAL.Entities;
using Synchronizer.DAL.Repositories;
using VK.Music.Service.Configuration;
using VK.Music.Service.Exceptions;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VK.Music.Service.Models.Auth;

public class RepositoryVkNetAuthService : IVkNetApiAuthService
{
    private readonly VkLinksRepository repository;
    private readonly ITwoFactorVkProvider twoFactorProvider;
    private readonly VkApiFactory vkApiFactory;
    private readonly VkServiceConfig vkServiceConfig;

    public RepositoryVkNetAuthService(
        VkLinksRepository repository,
        ITwoFactorVkProvider twoFactorProvider,
        VkApiFactory vkApiFactory,
        VkServiceConfig vkServiceConfig)
    {
        this.repository = repository;
        this.twoFactorProvider = twoFactorProvider;
        this.vkApiFactory = vkApiFactory;
        this.vkServiceConfig = vkServiceConfig;
    }

    public async Task<VkApi> AuthAsync(VkApi api, string username)
    {
        var foundLink = repository.Items.FirstOrDefault(x => x.Username == username);
        if (foundLink is not null)
        {
            try
            {
                await api.AuthorizeAsync(new ApiAuthParams
                {
                    AccessToken = foundLink.Token,
                    UserId = foundLink.VkUserId,
                    ApplicationId = vkServiceConfig.ApplicationId
                });
            }
            catch (Exception e)
            {
                await api.AuthorizeAsync(new ApiAuthParams
                {
                    Login = foundLink.Login,
                    Password = foundLink.Password,
                    Settings = Settings.Audio,
                    ApplicationId = vkServiceConfig.ApplicationId
                });
                foundLink.Token = api.Token;
                repository.Update(foundLink);
            }

            if (api.IsAuthorized) return api;
        }

        throw new AuthApiException("Вы не связали vk музыку");
    }

    public async Task CreateAuthSessionAsync(string username, string login, string password, string? code)
    {
        var vkNet = vkApiFactory.CreateApiClient();
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
        TwoFactorRequiredUsers.RequiredSecondFactor.TryRemove(username, out _);
        await repository.InsertAsync(new VkLink
        {
            VkUserId = vkNet.UserId!.Value,
            Password = password,
            Username = username,
            Login = login,
            Token = vkNet.Token
        });
    }
}