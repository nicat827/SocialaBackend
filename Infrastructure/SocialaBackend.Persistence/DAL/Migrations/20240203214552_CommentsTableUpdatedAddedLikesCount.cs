using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class CommentsTableUpdatedAddedLikesCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikeItems_AspNetUsers_AppUserId",
                table: "CommentLikeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikeItems_Comments_CommentId",
                table: "CommentLikeItems");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Replies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorImageUrl",
                table: "Replies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "CommentLikeItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "CommentLikeItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikeItems_AspNetUsers_AppUserId",
                table: "CommentLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikeItems_Comments_CommentId",
                table: "CommentLikeItems",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikeItems_AspNetUsers_AppUserId",
                table: "CommentLikeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikeItems_Comments_CommentId",
                table: "CommentLikeItems");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "AuthorImageUrl",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "LikesCount",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "CommentLikeItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "CommentLikeItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikeItems_AspNetUsers_AppUserId",
                table: "CommentLikeItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikeItems_Comments_CommentId",
                table: "CommentLikeItems",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
