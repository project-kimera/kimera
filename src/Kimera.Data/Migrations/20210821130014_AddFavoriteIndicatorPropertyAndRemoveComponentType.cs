using Microsoft.EntityFrameworkCore.Migrations;

namespace Kimera.Data.Migrations
{
    public partial class AddFavoriteIndicatorPropertyAndRemoveComponentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Components");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Components",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
