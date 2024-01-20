using Microsoft.Extensions.DependencyInjection;
using Synchronizer.Core.Services;
using Synchronizer.Core.VK;
using Synchronizer.Core.Yandex;
using Synchronizer.DAL;

namespace Synchronizer.Core.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSynchronizerCore(this IServiceCollection collection, SynchronizerConfig config)
    {
        collection.AddDataAccessLayer(config.DbConnection, config.MigrationsAssemly);
        collection.AddTransient<IUserService, UserService>();
        collection.AddTransient<VkMusicClient, VkMusicClient>();
        collection.AddTransient<YandexMusicClient, YandexMusicClient>();
        collection.AddTransient<ISynchronizerService, SynchronizerService>();
        return collection;
    }
}