using Microsoft.EntityFrameworkCore.Migrations;

namespace Kimera.Data.Migrations
{
    public partial class EditSomeProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Arguments",
                table: "PackageMetadatas",
                newName: "CommandLine");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "GameMetadatas",
                newName: "IsFinished");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommandLine",
                table: "PackageMetadatas",
                newName: "Arguments");

            migrationBuilder.RenameColumn(
                name: "IsFinished",
                table: "GameMetadatas",
                newName: "IsCompleted");
        }
    }
}
