using Kimera.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kimera.Data.Contexts
{
    public partial class KimeraContext : DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<CategorySubscription> CategorySubscriptions { get; set; }

        public virtual DbSet<Component> Components { get; set; }

        public virtual DbSet<Game> Games { get; set; }

        public virtual DbSet<GameMetadata> GameMetadatas { get; set; }

        public virtual DbSet<PackageMetadata> PackageMetadatas { get; set; }

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
                    .WithMany(p => p.CategorySubscriptions)
                    .HasPrincipalKey(p => p.SystemId)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CategorySubscription_Category");

                entity.HasOne(d => d.GameNavigation)
                    .WithMany(p => p.CategorySubscriptions)
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

                entity.HasOne(d => d.GameMetadataNavigation)
                    .WithOne(p => p.GameNavigation)
                    .HasPrincipalKey<Game>(p => p.SystemId)
                    .HasForeignKey<GameMetadata>(d => d.Game)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Game_GameMetadata");

                entity.HasOne(d => d.PackageMetadataNavigation)
                    .WithOne(p => p.GameNavigation)
                    .HasPrincipalKey<Game>(p => p.SystemId)
                    .HasForeignKey<PackageMetadata>(d => d.Game)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Game_PackageMetadata");
            });

            modelBuilder.Entity<GameMetadata>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_GameMetadata");

                entity.Property(e => e.Game)
                    .IsRequired()
                    .HasColumnType("TEXT");
            });

            modelBuilder.Entity<PackageMetadata>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_PackageMetadata");

                entity.Property(e => e.Game)
                    .IsRequired()
                    .HasColumnType("TEXT");

                entity.HasMany(d => d.Components)
                .WithOne(p => p.PackageMetadataNavigation)
                .HasPrincipalKey(p => p.SystemId)
                .HasForeignKey(d => d.PackageMetadata)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Component>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Component");

                entity.Property(e => e.PackageMetadata)
                    .IsRequired()
                    .HasColumnType("TEXT");
            });
        }
    }
}
