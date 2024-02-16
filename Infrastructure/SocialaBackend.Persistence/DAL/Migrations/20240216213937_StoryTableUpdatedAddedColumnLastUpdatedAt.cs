using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class StoryTableUpdatedAddedColumnLastUpdatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastItemAddedAt",
                table: "Stories",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastItemAddedAt",
                table: "Stories");
        }
    }
}
