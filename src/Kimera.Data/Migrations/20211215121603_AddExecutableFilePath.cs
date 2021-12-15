using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kimera.Data.Migrations
{
    public partial class AddExecutableFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExecutableFilePath",
                table: "PackageMetadatas",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutableFilePath",
                table: "PackageMetadatas");
        }
    }
}
