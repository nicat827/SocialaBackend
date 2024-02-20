using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class RenamedtableMessageMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageMedia_Messages_MessageId",
                table: "MessageMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageMedia",
                table: "MessageMedia");

            migrationBuilder.RenameTable(
                name: "MessageMedia",
                newName: "MessageMedias");

            migrationBuilder.RenameIndex(
                name: "IX_MessageMedia_MessageId",
                table: "MessageMedias",
                newName: "IX_MessageMedias_MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageMedias",
                table: "MessageMedias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageMedias_Messages_MessageId",
                table: "MessageMedias",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageMedias_Messages_MessageId",
                table: "MessageMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageMedias",
                table: "MessageMedias");

            migrationBuilder.RenameTable(
                name: "MessageMedias",
                newName: "MessageMedia");

            migrationBuilder.RenameIndex(
                name: "IX_MessageMedias_MessageId",
                table: "MessageMedia",
                newName: "IX_MessageMedia_MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageMedia",
                table: "MessageMedia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageMedia_Messages_MessageId",
                table: "MessageMedia",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
