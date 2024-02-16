using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class AppUserIsPersistenceAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPersistence",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPersistence",
                table: "AspNetUsers");
        }
    }
}
