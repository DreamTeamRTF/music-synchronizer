using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Synchronizer.DAL;
using Synchronizer.DAL.Entities;

namespace Synchronizer.Core.Extensions;

public static class ServiceProviderExtensions
{
    // Тут трабл, что это костыль и на несколько инстансов все рухнет
    public static void MigrateTables(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SynchronizerDbContext>();
        if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

        if (context.Roles.Count() != 2)
        {
            context.Roles.Add(new Role { Id = 1, Name = "User" });
            context.Roles.Add(new Role { Id = 2, Name = "Admin" });
            context.SaveChanges();
        }
    }
}