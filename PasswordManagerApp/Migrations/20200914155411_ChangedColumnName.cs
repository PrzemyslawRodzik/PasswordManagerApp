using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordManagerApp.Migrations
{
    public partial class ChangedColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_address",
                table: "user_device",
                newName: "ip_address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "user_device",
                newName: "id_address");
        }
    }
}
