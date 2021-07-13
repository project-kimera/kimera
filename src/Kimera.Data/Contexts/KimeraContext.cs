using Kimera.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kimera.Data.Contexts
{
    public partial class KimeraContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }

        public virtual DbSet<CategorySubscription> CategorySubscription { get; set; }

        public virtual DbSet<Game> Game { get; set; }

        public virtual DbSet<GameMetadata> GameMetadata { get; set; }

        public virtual DbSet<PackageMetadata> PackageMetadata { get; set; }

        public virtual DbSet<Plugin> Plugin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.SystemId)
                    .HasDatabaseName("IX_Category")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("TEXT");
            });

            modelBuilder.Entity<CategorySubscription>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_CategorySubscription");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.CategorySubscription)
                    .HasPrincipalKey(p => p.SystemId)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CategorySubscription_Category");

                entity.HasOne(d => d.GameNavigation)
                    .WithMany(p => p.CategorySubscription)
                    .HasPrincipalKey(p => p.SystemId)
                    .HasForeignKey(d => d.Game)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CategorySubscription_Game");
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasIndex(e => e.SystemId)
                    .HasDatabaseName("IX_Game")
                    .IsUnique();

                entity.Property(e => e.GameMetadata)
                    .IsRequired()
                    .HasColumnType("TEXT");

                entity.Property(e => e.PackageMetadata)
                    .IsRequired()
                    .HasColumnType("TEXT");
            });

            modelBuilder.Entity<GameMetadata>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_GameMetadata");

                entity.Property(e => e.Game)
                    .IsRequired()
                    .HasColumnType("TEXT");

                entity.HasOne(d => d.GameNavigation)
                    .WithOne(p => p.GameMetadataNavigation)
                    .HasPrincipalKey<GameMetadata>(p => p.Game)
                    .HasForeignKey<Game>(d => d.GameMetadata)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_GameMetadata_Game");
            });

            modelBuilder.Entity<PackageMetadata>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_PackageMetadata");

                entity.Property(e => e.Game)
                    .IsRequired()
                    .HasColumnType("TEXT");

                entity.HasOne(d => d.GameNavigation)
                    .WithOne(p => p.PackageMetadataNavigation)
                    .HasPrincipalKey<PackageMetadata>(p => p.Game)
                    .HasForeignKey<Game>(d => d.PackageMetadata)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PackageMetadata_Game");

            });
        }
    }
}
