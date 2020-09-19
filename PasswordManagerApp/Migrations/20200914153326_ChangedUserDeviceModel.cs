using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordManagerApp.Migrations
{
    public partial class ChangedUserDeviceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "id_address",
                table: "user_device",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_address",
                table: "user_device");
        }
    }
}
