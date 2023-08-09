using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMessageLogger.Migrations
{
    /// <inheritdoc />
    public partial class addedUsertoMessageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_user_id",
                table: "messages");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "messages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_user_id",
                table: "messages",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_user_id",
                table: "messages");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "messages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_user_id",
                table: "messages",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
