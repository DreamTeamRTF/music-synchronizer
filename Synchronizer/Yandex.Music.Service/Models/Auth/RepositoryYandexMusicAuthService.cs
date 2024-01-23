using Microsoft.EntityFrameworkCore;
using Synchronizer.DAL.Entities;
using Synchronizer.DAL.Repositories;
using Yandex.Music.Client;
using Yandex.Music.Service.Exceptions;

namespace Yandex.Music.Service.Models.Auth;

public class RepositoryYandexMusicAuthService
{
    private readonly YandexLinksRepository repository;
    private readonly ILogger<RepositoryYandexMusicAuthService> logger;

    public RepositoryYandexMusicAuthService(YandexLinksRepository repository, ILogger<RepositoryYandexMusicAuthService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }
    
    public async Task CreateAuthSessionAsync(string username, string login, string password)
    {
        var yandex = YandexApiFactory.CreateApiClient();
        await yandex.CreateAuthSession(login);
        await yandex.AuthorizeByAppPassword(password);
        var token = await yandex.GetAccessToken();
        logger.LogInformation("Added {Username} to auth sessions", username);
        var yandexLink = repository.Items.FirstOrDefault(x => x.Username == username || x.Login == login);
        if (yandexLink == null)
        {
            await repository.InsertAsync(new YandexLink
            {
                Login = login,
                Password = password,
                Token = token.AccessToken,
                Username = username
            });
        }
        else
        {
            yandexLink.Token = token.AccessToken;
            repository.Update(yandexLink);
        }
    }

    public async Task<YandexMusicClientAsync> AuthAsync(string username)
    {
        var api = YandexApiFactory.CreateApiClient();
        var authLink = repository.Items.FirstOrDefault(x => x.Username == username);
        if (authLink is not null)
        {
            try
            {
                await api.Authorize(authLink.Token);
            }
            catch (Exception e)
            {
                api = YandexApiFactory.CreateApiClient();
                logger.LogInformation("YA Session has expired for {Username}, {E}", username, e);
                await api.CreateAuthSession(authLink.Login);
                await api.AuthorizeByAppPassword(authLink.Password);
                var token = await api.GetAccessToken();
                authLink.Token = token.AccessToken;
                repository.Update(authLink);
            }
            
            if (api.IsAuthorized) return api;
        }

        throw new AuthApiException("Something went wrong in auth");
    }
}