using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class AppUserVerifyRequestAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VerifyRequests_AppUserId",
                table: "VerifyRequests");

            migrationBuilder.CreateIndex(
                name: "IX_VerifyRequests_AppUserId",
                table: "VerifyRequests",
                column: "AppUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VerifyRequests_AppUserId",
                table: "VerifyRequests");

            migrationBuilder.CreateIndex(
                name: "IX_VerifyRequests_AppUserId",
                table: "VerifyRequests",
                column: "AppUserId");
        }
    }
}
