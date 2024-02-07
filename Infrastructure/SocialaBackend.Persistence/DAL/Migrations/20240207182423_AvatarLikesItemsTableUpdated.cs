using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class AvatarLikesItemsTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvatarLikeItems_AspNetUsers_AppUserId",
                table: "AvatarLikeItems");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "AvatarLikeItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AvatarLikeItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AvatarLikeItems_AspNetUsers_AppUserId",
                table: "AvatarLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvatarLikeItems_AspNetUsers_AppUserId",
                table: "AvatarLikeItems");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AvatarLikeItems");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "AvatarLikeItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_AvatarLikeItems_AspNetUsers_AppUserId",
                table: "AvatarLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
