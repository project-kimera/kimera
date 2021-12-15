﻿// <auto-generated />
using System;
using Kimera.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kimera.Data.Migrations
{
    [DbContext(typeof(KimeraContext))]
    [Migration("20211215121603_AddExecutableFilePath")]
    partial class AddExecutableFilePath
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("Kimera.Data.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SystemId")
                        .IsUnique()
                        .HasDatabaseName("IX_Category");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Kimera.Data.Entities.CategorySubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Category")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Game")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_CategorySubscription");

                    b.HasIndex("Category");

                    b.HasIndex("Game");

                    b.ToTable("CategorySubscriptions");
                });

            modelBuilder.Entity("Kimera.Data.Entities.Component", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OffsetPath")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PackageMetadata")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_Component");

                    b.HasIndex("PackageMetadata");

                    b.ToTable("Components");
                });

            modelBuilder.Entity("Kimera.Data.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("GameMetadata")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("PackageMetadata")
                        .HasColumnType("TEXT");

                    b.Property<int>("PackageStatus")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SystemId")
                        .IsUnique()
                        .HasDatabaseName("IX_Game");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Kimera.Data.Entities.GameMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdmittedAge")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Creator")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FirstTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Game")
                        .HasColumnType("TEXT");

                    b.Property<string>("Genres")
                        .HasColumnType("TEXT");

                    b.Property<string>("HomepageUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("IconUri")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Memo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayTime")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Score")
                        .HasColumnType("REAL");

                    b.Property<string>("SupportedLanguages")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tags")
                        .HasColumnType("TEXT");

                    b.Property<string>("ThumbnailUri")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_GameMetadata");

                    b.HasIndex("Game")
                        .IsUnique();

                    b.ToTable("GameMetadatas");
                });

            modelBuilder.Entity("Kimera.Data.Entities.PackageMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandLineArguments")
                        .HasColumnType("TEXT");

                    b.Property<string>("EntryPointFilePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExecutableFilePath")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Game")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id")
                        .HasName("PK_PackageMetadata");

                    b.HasIndex("Game")
                        .IsUnique();

                    b.ToTable("PackageMetadatas");
                });

            modelBuilder.Entity("Kimera.Data.Entities.CategorySubscription", b =>
                {
                    b.HasOne("Kimera.Data.Entities.Category", "CategoryNavigation")
                        .WithMany("CategorySubscriptions")
                        .HasForeignKey("Category")
                        .HasPrincipalKey("SystemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_CategorySubscription_Category");

                    b.HasOne("Kimera.Data.Entities.Game", "GameNavigation")
                        .WithMany("CategorySubscriptions")
                        .HasForeignKey("Game")
                        .HasPrincipalKey("SystemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_CategorySubscription_Game");

                    b.Navigation("CategoryNavigation");

                    b.Navigation("GameNavigation");
                });

            modelBuilder.Entity("Kimera.Data.Entities.Component", b =>
                {
                    b.HasOne("Kimera.Data.Entities.PackageMetadata", "PackageMetadataNavigation")
                        .WithMany("Components")
                        .HasForeignKey("PackageMetadata")
                        .HasPrincipalKey("SystemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PackageMetadataNavigation");
                });

            modelBuilder.Entity("Kimera.Data.Entities.GameMetadata", b =>
                {
                    b.HasOne("Kimera.Data.Entities.Game", "GameNavigation")
                        .WithOne("GameMetadataNavigation")
                        .HasForeignKey("Kimera.Data.Entities.GameMetadata", "Game")
                        .HasPrincipalKey("Kimera.Data.Entities.Game", "SystemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Game_GameMetadata");

                    b.Navigation("GameNavigation");
                });

            modelBuilder.Entity("Kimera.Data.Entities.PackageMetadata", b =>
                {
                    b.HasOne("Kimera.Data.Entities.Game", "GameNavigation")
                        .WithOne("PackageMetadataNavigation")
                        .HasForeignKey("Kimera.Data.Entities.PackageMetadata", "Game")
                        .HasPrincipalKey("Kimera.Data.Entities.Game", "SystemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Game_PackageMetadata");

                    b.Navigation("GameNavigation");
                });

            modelBuilder.Entity("Kimera.Data.Entities.Category", b =>
                {
                    b.Navigation("CategorySubscriptions");
                });

            modelBuilder.Entity("Kimera.Data.Entities.Game", b =>
                {
                    b.Navigation("CategorySubscriptions");

                    b.Navigation("GameMetadataNavigation");

                    b.Navigation("PackageMetadataNavigation");
                });

            modelBuilder.Entity("Kimera.Data.Entities.PackageMetadata", b =>
                {
                    b.Navigation("Components");
                });
#pragma warning restore 612, 618
        }
    }
}
