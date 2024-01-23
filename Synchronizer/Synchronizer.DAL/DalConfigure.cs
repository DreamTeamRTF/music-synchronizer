using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Synchronizer.DAL.Repositories;

namespace Synchronizer.DAL;

public static class DalConfigure
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection collection, string dbConnection,
        string assemblyName)
    {
        collection.AddDbContext<SynchronizerDbContext>(options => options
            .UseNpgsql(dbConnection, b => b.MigrationsAssembly(assemblyName)));
        collection.AddTransient<IUserRepository, UserRepository>();
        collection.AddTransient<SynchronizedPlaylistsRepository>();
        collection.AddTransient<YandexLinksRepository>();
        collection.AddTransient<VkLinksRepository>();
        return collection;
    }
}