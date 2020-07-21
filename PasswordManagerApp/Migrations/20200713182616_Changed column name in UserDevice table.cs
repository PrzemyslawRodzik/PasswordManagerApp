using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordManagerApp.Migrations
{
    public partial class ChangedcolumnnameinUserDevicetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CookieDeviceHash",
                table: "user_device");

            migrationBuilder.AddColumn<string>(
                name: "DeviceGuid",
                table: "user_device",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceGuid",
                table: "user_device");

            migrationBuilder.AddColumn<string>(
                name: "CookieDeviceHash",
                table: "user_device",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
