using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class StoryItemTableChangedAddedText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Story_AspNetUsers_AppUserId",
                table: "Story");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Story",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Story_AppUserId",
                table: "Story",
                newName: "IX_Story_OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "StoryItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Story_AspNetUsers_OwnerId",
                table: "Story",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Story_AspNetUsers_OwnerId",
                table: "Story");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "StoryItem");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Story",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Story_OwnerId",
                table: "Story",
                newName: "IX_Story_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Story_AspNetUsers_AppUserId",
                table: "Story",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
