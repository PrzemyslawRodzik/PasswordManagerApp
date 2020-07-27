using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PasswordManagerApp.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "breached_password",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    password = table.Column<string>(nullable: false),
                    occuring = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_breached_password", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(nullable: false),
                    master_password = table.Column<string>(nullable: false),
                    password_salt = table.Column<string>(nullable: false),
                    two_factor_authorization = table.Column<int>(nullable: false),
                    admin = table.Column<int>(nullable: false),
                    password_notifications = table.Column<int>(nullable: false),
                    authentication_time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "visitor_agent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    device = table.Column<string>(nullable: false),
                    browser = table.Column<string>(nullable: false),
                    operating_system = table.Column<string>(nullable: false),
                    visit_time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitor_agent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "credit_card",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: false),
                    cardholder_name = table.Column<string>(nullable: false),
                    card_number = table.Column<string>(nullable: false),
                    security_code = table.Column<string>(nullable: false),
                    expiration_date = table.Column<string>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_credit_card_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login_data",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: true),
                    login = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: false),
                    website = table.Column<string>(nullable: true),
                    compromised = table.Column<int>(nullable: false),
                    modified_date = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_login_data_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "note",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: false),
                    details = table.Column<string>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_note_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "paypall_account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: false),
                    compromised = table.Column<int>(nullable: false),
                    modified_date = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paypall_account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_paypall_account_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personal_info",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: true),
                    second_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "Date", nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal_info", x => x.Id);
                    table.ForeignKey(
                        name: "FK_personal_info_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Totp_Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    token = table.Column<string>(nullable: false),
                    create_date = table.Column<DateTime>(nullable: false),
                    expire_date = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Totp_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Totp_Users_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_device",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    device_guid = table.Column<string>(nullable: false),
                    authorized = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_device_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shared_login_data",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    login_data_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shared_login_data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shared_login_data_login_data_login_data_id",
                        column: x => x.login_data_id,
                        principalTable: "login_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shared_login_data_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    address_name = table.Column<string>(nullable: false),
                    street = table.Column<string>(nullable: false),
                    zip_code = table.Column<string>(nullable: false),
                    city = table.Column<string>(nullable: false),
                    country = table.Column<string>(nullable: false),
                    personal_info_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_address_personal_info_personal_info_id",
                        column: x => x.personal_info_id,
                        principalTable: "personal_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phone_number",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nickname = table.Column<string>(nullable: false),
                    phone_number = table.Column<string>(nullable: false),
                    type = table.Column<string>(nullable: false),
                    personal_info_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_number", x => x.Id);
                    table.ForeignKey(
                        name: "FK_phone_number_personal_info_personal_info_id",
                        column: x => x.personal_info_id,
                        principalTable: "personal_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_personal_info_id",
                table: "address",
                column: "personal_info_id");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_user_id",
                table: "credit_card",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_login_data_user_id",
                table: "login_data",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_user_id",
                table: "note",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_paypall_account_user_id",
                table: "paypall_account",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_personal_info_user_id",
                table: "personal_info",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_phone_number_personal_info_id",
                table: "phone_number",
                column: "personal_info_id");

            migrationBuilder.CreateIndex(
                name: "IX_shared_login_data_login_data_id",
                table: "shared_login_data",
                column: "login_data_id");

            migrationBuilder.CreateIndex(
                name: "IX_shared_login_data_user_id",
                table: "shared_login_data",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Totp_Users_user_id",
                table: "Totp_Users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_device_user_id",
                table: "user_device",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "breached_password");

            migrationBuilder.DropTable(
                name: "credit_card");

            migrationBuilder.DropTable(
                name: "note");

            migrationBuilder.DropTable(
                name: "paypall_account");

            migrationBuilder.DropTable(
                name: "phone_number");

            migrationBuilder.DropTable(
                name: "shared_login_data");

            migrationBuilder.DropTable(
                name: "Totp_Users");

            migrationBuilder.DropTable(
                name: "user_device");

            migrationBuilder.DropTable(
                name: "visitor_agent");

            migrationBuilder.DropTable(
                name: "personal_info");

            migrationBuilder.DropTable(
                name: "login_data");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
