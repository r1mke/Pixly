using backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet <Tag> Tags { get; set; }
        public DbSet <PhotoTag> PhotoTags { get; set; }
        public DbSet <PhotoResolution> PhotoResolutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>()
                .HasOne(p => p.User)
                .WithMany(u=>u.Photos)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhotoTag>()
                .HasKey(pt => new { pt.PhotoId, pt.TagId });

            modelBuilder.Entity<PhotoTag>()
                .HasOne(pt => pt.Photo)
                .WithMany(p => p.PhotoTags)
                .HasForeignKey(pt => pt.PhotoId);

            modelBuilder.Entity<PhotoTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PhotoTags)
                .HasForeignKey(pt => pt.TagId);

            modelBuilder.Entity<PhotoResolution>()
                .HasOne(pr => pr.Photo)
               .WithMany(p => p.Resolutions)
               .HasForeignKey(pr => pr.PhotoId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
