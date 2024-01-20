using Microsoft.EntityFrameworkCore;
using Synchronizer.DAL.Entities;

namespace Synchronizer.DAL;

public class SynchronizerDbContext : DbContext
{
    public SynchronizerDbContext(DbContextOptions<SynchronizerDbContext> options) : base(options)
    {
    }

    public DbSet<PlaylistEntity> Playlists { get; set; }
    public DbSet<SynchronizedPlaylistLink> Links { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlaylistEntity>()
            .HasMany(e => e.Tracks)
            .WithOne(e => e.PlaylistEntity)
            .HasForeignKey(e => e.PlaylistId)
            .IsRequired();

        modelBuilder.Entity<Track>()
            .Property(t => t.MusicService)
            .HasConversion(v => v.ToString(),
                v => (MusicServiceType)Enum.Parse(typeof(MusicServiceType), v)).IsUnicode(false);

        modelBuilder.Entity<PlaylistEntity>()
            .Property(p => p.MusicService)
            .HasConversion(v => v.ToString(),
                v => (MusicServiceType)Enum.Parse(typeof(MusicServiceType), v)).IsUnicode(false);
        
        modelBuilder.Entity<SynchronizedPlaylistLink>()
            .Property(p => p.MainMusicService)
            .HasConversion(v => v.ToString(),
                v => (MusicServiceType)Enum.Parse(typeof(MusicServiceType), v)).IsUnicode(false);
    }
}