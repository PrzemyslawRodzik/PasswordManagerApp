using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordManagerApp.Migrations
{
    public partial class KeysfieldtoUsermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "note",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "details",
                table: "note",
                newName: "Details");

            migrationBuilder.AddColumn<string>(
                name: "private_key",
                table: "user",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "public_key",
                table: "user",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "private_key",
                table: "user");

            migrationBuilder.DropColumn(
                name: "public_key",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "note",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "note",
                newName: "details");
        }
    }
}
