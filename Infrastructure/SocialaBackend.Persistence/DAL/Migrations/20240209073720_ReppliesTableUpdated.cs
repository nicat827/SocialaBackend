using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class ReppliesTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "AuthorImageUrl",
                table: "Replies");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Replies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_AuthorId",
                table: "Replies",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_AspNetUsers_AuthorId",
                table: "Replies",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_AspNetUsers_AuthorId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_AuthorId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Replies");

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
        }
    }
}
