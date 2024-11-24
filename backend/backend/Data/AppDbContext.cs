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
        public DbSet <EmailVerificationCode> EmailVerificationCodes { get; set; }
        public DbSet <Color> Colors { get; set; }
        public DbSet<PhotoColor> PhotoColors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PhotoCategory> PhotoCategories { get; set; }

        public DbSet<Like> Likes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>()
                .HasOne(p => p.User)
                .WithMany(u => u.Photos)
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

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<EmailVerificationCode>()
                .HasIndex(evc => new { evc.UserId, evc.ActivateCode })
                .IsUnique();

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.PhotoId })
                .IsUnique();

            modelBuilder.Entity<PhotoColor>()
                .HasKey(pc => new { pc.PhotoId, pc.ColorId });

            modelBuilder.Entity<PhotoColor>()
                .HasOne(pc => pc.Photo)
                .WithMany(p => p.PhotoColors)
                .HasForeignKey(pc => pc.PhotoId);

            modelBuilder.Entity<PhotoColor>()
                .HasOne(pc => pc.Color)
                .WithMany(c => c.PhotoColors)
                .HasForeignKey(pc => pc.ColorId);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhotoCategory>()
                .HasKey(pc => new { pc.PhotoId, pc.CategoryId }); // Kombinirani ključ

            modelBuilder.Entity<PhotoCategory>()
                .HasOne(pc => pc.Photo)
                .WithMany(p => p.PhotoCategories)
                .HasForeignKey(pc => pc.PhotoId);

            modelBuilder.Entity<PhotoCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.PhotoCategories)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
