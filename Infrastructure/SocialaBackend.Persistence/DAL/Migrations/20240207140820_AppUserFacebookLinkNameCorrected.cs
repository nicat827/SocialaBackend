using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class AppUserFacebookLinkNameCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeacebookLink",
                table: "AspNetUsers",
                newName: "FacebookLink");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FacebookLink",
                table: "AspNetUsers",
                newName: "FeacebookLink");
        }
    }
}
