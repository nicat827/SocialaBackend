using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class StoriesTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "StoryItems",
                type: "nvarchar(max)",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Text",
                table: "StoryItems");

        }
    }
}
