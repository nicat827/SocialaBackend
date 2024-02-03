using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class PostEntityUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostItem_Posts_PostId",
                table: "PostItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostItem",
                table: "PostItem");

            migrationBuilder.RenameTable(
                name: "PostItem",
                newName: "PostItems");

            migrationBuilder.RenameIndex(
                name: "IX_PostItem_PostId",
                table: "PostItems",
                newName: "IX_PostItems_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostItems",
                table: "PostItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostItems_Posts_PostId",
                table: "PostItems",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostItems_Posts_PostId",
                table: "PostItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostItems",
                table: "PostItems");

            migrationBuilder.RenameTable(
                name: "PostItems",
                newName: "PostItem");

            migrationBuilder.RenameIndex(
                name: "IX_PostItems_PostId",
                table: "PostItem",
                newName: "IX_PostItem_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostItem",
                table: "PostItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostItem_Posts_PostId",
                table: "PostItem",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
