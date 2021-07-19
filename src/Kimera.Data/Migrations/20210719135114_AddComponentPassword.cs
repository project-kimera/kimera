using Microsoft.EntityFrameworkCore.Migrations;

namespace Kimera.Data.Migrations
{
    public partial class AddComponentPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Components",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Components");
        }
    }
}
