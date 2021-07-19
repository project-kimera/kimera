using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kimera.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SystemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.UniqueConstraint("AK_Categories_SystemId", x => x.SystemId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SystemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameMetadata = table.Column<Guid>(type: "TEXT", nullable: false),
                    PackageMetadata = table.Column<Guid>(type: "TEXT", nullable: false),
                    PackageStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.UniqueConstraint("AK_Games_SystemId", x => x.SystemId);
                });

            migrationBuilder.CreateTable(
                name: "CategorySubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<Guid>(type: "TEXT", nullable: false),
                    Game = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategorySubscription_Category",
                        column: x => x.Category,
                        principalTable: "Categories",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategorySubscription_Game",
                        column: x => x.Game,
                        principalTable: "Games",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameMetadatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SystemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Game = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Creator = table.Column<string>(type: "TEXT", nullable: true),
                    AdmittedAge = table.Column<int>(type: "INTEGER", nullable: false),
                    Genres = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    SupportedLanguages = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<double>(type: "REAL", nullable: false),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    PlayTime = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IconUri = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailUri = table.Column<string>(type: "TEXT", nullable: true),
                    HomepageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_GameMetadata",
                        column: x => x.Game,
                        principalTable: "Games",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackageMetadatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SystemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Game = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryPointFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    Arguments = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageMetadata", x => x.Id);
                    table.UniqueConstraint("AK_PackageMetadatas_SystemId", x => x.SystemId);
                    table.ForeignKey(
                        name: "FK_Game_PackageMetadata",
                        column: x => x.Game,
                        principalTable: "Games",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageMetadata = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Component", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_PackageMetadatas_PackageMetadata",
                        column: x => x.PackageMetadata,
                        principalTable: "PackageMetadatas",
                        principalColumn: "SystemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category",
                table: "Categories",
                column: "SystemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubscriptions_Category",
                table: "CategorySubscriptions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySubscriptions_Game",
                table: "CategorySubscriptions",
                column: "Game");

            migrationBuilder.CreateIndex(
                name: "IX_Components_PackageMetadata",
                table: "Components",
                column: "PackageMetadata");

            migrationBuilder.CreateIndex(
                name: "IX_GameMetadatas_Game",
                table: "GameMetadatas",
                column: "Game",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game",
                table: "Games",
                column: "SystemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageMetadatas_Game",
                table: "PackageMetadatas",
                column: "Game",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorySubscriptions");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "GameMetadatas");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "PackageMetadatas");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
