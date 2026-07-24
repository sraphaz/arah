using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arah.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserEmailNullableAndUserDeviceFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(320)",
                oldMaxLength: 320,
                oldNullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_user_devices_users_UserId",
                table: "user_devices",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_devices_users_UserId",
                table: "user_devices");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(320)",
                oldMaxLength: 320,
                oldNullable: true);
        }
    }
}
