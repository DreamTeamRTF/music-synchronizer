using Autofac;
using MusicServices.Models.Contracts;
using Synchronizer.DAL.Repositories;
using VK.Music.Service.Configuration;
using VK.Music.Service.Helpers;
using VK.Music.Service.Models;
using VK.Music.Service.Models.Account;
using VK.Music.Service.Models.Auth;
using VK.Music.Service.Models.Music;
using VkNet.AudioBypassService.Extensions;
using VkNet.Utils.AntiCaptcha;

namespace VK.Music.Service.DI;

public class VkServiceModule : Module
{
    private readonly VkServiceConfig config;

    public VkServiceModule(VkServiceConfig config)
    {
        this.config = config;
    }

    protected override void Load(ContainerBuilder containerBuilder)
    {
        var factory = LoggerFactory.Create(x => x.AddConsole());
        containerBuilder.Register(_ => factory.CreateLogger("vk_app")).As<ILogger>().SingleInstance();
        containerBuilder.Register(_ => new ConsoleVkCaptchaSolver())
            .As<ICaptchaSolver>()
            .SingleInstance();

        containerBuilder.Register(cc => config)
            .As<VkServiceConfig>()
            .SingleInstance();

        containerBuilder.Register(cc => new DefaultTwoFactorProvider())
            .As<ITwoFactorVkProvider>()
            .SingleInstance();

        var serviceCollection = new ServiceCollection(); // для AudioBypass расширения
        serviceCollection.AddSingleton<ILogger>(_ => factory.CreateLogger("vk_bypass"));
        serviceCollection.AddSingleton<ICaptchaSolver>(_ => new ConsoleVkCaptchaSolver());
        serviceCollection.AddAudioBypass();

        containerBuilder.Register<VkApiFactory>(cc => new VkApiFactory(serviceCollection))
            .As<VkApiFactory>()
            .SingleInstance();

        containerBuilder.Register(cc => new RepositoryVkNetAuthService(
                cc.Resolve<VkLinksRepository>(),
                cc.Resolve<ITwoFactorVkProvider>(),
                cc.Resolve<VkApiFactory>(),
                cc.Resolve<VkServiceConfig>()))
            .As<IVkNetApiAuthService>()
            .SingleInstance();

        containerBuilder.Register(cc => new VkNetClientsRepository(
                cc.Resolve<IVkNetApiAuthService>(),
                cc.Resolve<VkApiFactory>()))
            .As<IVkNetClientsRepository>()
            .SingleInstance();

        containerBuilder
            .Register(cc => new VkNetApiClient(cc.Resolve<IVkNetClientsRepository>()))
            .As<IVkNetApiClient>()
            .SingleInstance();

        containerBuilder
            .Register(cc => new VkAccountService(cc.Resolve<IVkNetApiClient>()))
            .As<IVkAccountService>()
            .SingleInstance();

        containerBuilder
            .Register(cc => new VkMusicService(cc.Resolve<IVkNetApiClient>()))
            .As<IMusicService>()
            .SingleInstance();
    }
}