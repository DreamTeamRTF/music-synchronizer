using Autofac;
using MusicServices.Models.Contracts;
using Yandex.Music.Service.Configuration;
using Yandex.Music.Service.Models;
using Yandex.Music.Service.Models.Account;
using Yandex.Music.Service.Models.Auth;
using Yandex.Music.Service.Models.Music;

namespace Yandex.Music.Service.DI;

public class YandexServiceModule : Module
{
    private readonly YandexServiceConfig config;

    public YandexServiceModule(YandexServiceConfig config)
    {
        this.config = config;
    }

    protected override void Load(ContainerBuilder containerBuilder)
    {
        var factory = LoggerFactory.Create(x => x.AddConsole());
        containerBuilder.Register(_ => factory.CreateLogger("yandex_app"))
            .As<ILogger>().SingleInstance();

        containerBuilder.Register(cc => config)
            .As<YandexServiceConfig>()
            .SingleInstance();

        containerBuilder.Register(cc =>
                new InMemoryYandexMusicAuthService(cc.Resolve<ILogger<InMemoryYandexMusicAuthService>>()))
            .As<InMemoryYandexMusicAuthService>()
            .SingleInstance();

        containerBuilder.Register(cc => new YandexClientsRepository(
                cc.Resolve<InMemoryYandexMusicAuthService>()))
            .As<YandexClientsRepository>()
            .SingleInstance();

        containerBuilder.Register(cc => new YandexApiClient(
                cc.Resolve<YandexClientsRepository>(),
                cc.Resolve<ILogger<YandexApiClient>>()))
            .As<YandexApiClient>()
            .SingleInstance();

        containerBuilder.Register(cc => new YandexAccountService(
                cc.Resolve<YandexApiClient>()))
            .As<YandexAccountService>()
            .SingleInstance();

        containerBuilder.Register(cc => new YandexMusicService(
                cc.Resolve<YandexApiClient>()))
            .As<IMusicService>()
            .SingleInstance();
    }
}