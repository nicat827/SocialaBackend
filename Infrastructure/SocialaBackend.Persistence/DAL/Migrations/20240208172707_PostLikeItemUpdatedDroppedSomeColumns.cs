using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class PostLikeItemUpdatedDroppedSomeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_AppUserId",
                table: "PostLikeItems");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "PostLikeItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PostLikeItems");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "PostLikeItems");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "PostLikeItems");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "PostLikeItems",
                newName: "LikedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLikeItems_AppUserId",
                table: "PostLikeItems",
                newName: "IX_PostLikeItems_LikedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_LikedUserId",
                table: "PostLikeItems",
                column: "LikedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_LikedUserId",
                table: "PostLikeItems");

            migrationBuilder.RenameColumn(
                name: "LikedUserId",
                table: "PostLikeItems",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLikeItems_LikedUserId",
                table: "PostLikeItems",
                newName: "IX_PostLikeItems_AppUserId");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "PostLikeItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PostLikeItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "PostLikeItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "PostLikeItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_AppUserId",
                table: "PostLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
