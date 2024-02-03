using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class CommentsTableUpdatedChangedNamesForAuthorAndAuthorImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserUsername",
                table: "Comments",
                newName: "Author");

            migrationBuilder.RenameColumn(
                name: "UserImageUrl",
                table: "Comments",
                newName: "AuthorImageUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorImageUrl",
                table: "Comments",
                newName: "UserImageUrl");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Comments",
                newName: "UserUsername");
        }
    }
}
