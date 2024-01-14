using Autofac;
using MusicServices.Models.Contracts;
using Yandex.Music.Service.Configuration;
using Yandex.Music.Service.Helpers;
using Yandex.Music.Service.Models;
using Yandex.Music.Service.Models.Auth;

namespace Yandex.Music.Service.DI
{
    public class YandexServiceModule : Module
    {
        private readonly YandexServiceConfig _config;

        public YandexServiceModule(YandexServiceConfig config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            var factory = LoggerFactory.Create(x => x.AddConsole());
            containerBuilder.Register(_ => factory.CreateLogger("yandex_app")).As<ILogger>().SingleInstance();
            containerBuilder.Register(_ => new ConsoleYandexCaptchaSolver())
                .SingleInstance();

            containerBuilder.Register(cc => _config)
            .As<YandexServiceConfig>()
            .SingleInstance();
        }
    }
}
