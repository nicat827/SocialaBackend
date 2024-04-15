using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class MessagesTableUpdatedDeletedSecondsAndMinutesAddedSourceUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Minutes",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "Minutes",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "Messages",
                type: "int",
                nullable: true);
        }
    }
}
