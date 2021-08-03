using Microsoft.EntityFrameworkCore.Migrations;

namespace Kimera.Data.Migrations
{
    public partial class AddComponentOffsetPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OffsetPath",
                table: "Components",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffsetPath",
                table: "Components");
        }
    }
}
