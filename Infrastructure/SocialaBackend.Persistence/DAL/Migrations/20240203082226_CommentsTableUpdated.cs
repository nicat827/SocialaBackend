using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class CommentsTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserImageUrl",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUsername",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImageUrl",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserUsername",
                table: "Comments");
        }
    }
}
