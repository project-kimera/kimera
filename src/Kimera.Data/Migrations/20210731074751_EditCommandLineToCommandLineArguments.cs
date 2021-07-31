using Microsoft.EntityFrameworkCore.Migrations;

namespace Kimera.Data.Migrations
{
    public partial class EditCommandLineToCommandLineArguments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommandLine",
                table: "PackageMetadatas",
                newName: "CommandLineArguments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommandLineArguments",
                table: "PackageMetadatas",
                newName: "CommandLine");
        }
    }
}
