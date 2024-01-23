using Microsoft.EntityFrameworkCore;
using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL;

public class SynchronizerDbContext : DbContext
{
    public SynchronizerDbContext(DbContextOptions<SynchronizerDbContext> options) : base(options)
    {
    }

    public DbSet<SynchronizedPlaylistLink> Links { get; set; }
    public DbSet<SyncTrack> SyncTracks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<YandexLink> YandexLinks { get; set; }
    public DbSet<VkLink> VkLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SynchronizedPlaylistLink>()
            .Property(p => p.MainMusicService)
            .HasConversion(v => v.ToString(),
                v => (MusicServiceType)Enum.Parse(typeof(MusicServiceType), v)).IsUnicode(false);
        modelBuilder.Entity<SynchronizedPlaylistLink>()
            .HasMany(p => p.Tracks)
            .WithOne(x => x.Link);
    }
}