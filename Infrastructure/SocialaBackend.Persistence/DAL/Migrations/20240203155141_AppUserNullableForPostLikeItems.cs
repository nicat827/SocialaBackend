using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class AppUserNullableForPostLikeItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_AppUserId",
                table: "PostLikeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikeItems_Posts_PostId",
                table: "PostLikeItems");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "PostLikeItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_AppUserId",
                table: "PostLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikeItems_Posts_PostId",
                table: "PostLikeItems",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_AppUserId",
                table: "PostLikeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikeItems_Posts_PostId",
                table: "PostLikeItems");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "PostLikeItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikeItems_AspNetUsers_AppUserId",
                table: "PostLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikeItems_Posts_PostId",
                table: "PostLikeItems",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
