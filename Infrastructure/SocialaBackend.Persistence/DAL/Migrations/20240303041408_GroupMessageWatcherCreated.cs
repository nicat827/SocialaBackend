using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class GroupMessageWatcherCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GroupMessages_GroupMessageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GroupMessageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GroupMessageId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "GroupMessageWatcher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupMessageId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMessageWatcher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMessageWatcher_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupMessageWatcher_GroupMessages_GroupMessageId",
                        column: x => x.GroupMessageId,
                        principalTable: "GroupMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessageWatcher_AppUserId",
                table: "GroupMessageWatcher",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessageWatcher_GroupMessageId",
                table: "GroupMessageWatcher",
                column: "GroupMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupMessageWatcher");

            migrationBuilder.AddColumn<int>(
                name: "GroupMessageId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GroupMessageId",
                table: "AspNetUsers",
                column: "GroupMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GroupMessages_GroupMessageId",
                table: "AspNetUsers",
                column: "GroupMessageId",
                principalTable: "GroupMessages",
                principalColumn: "Id");
        }
    }
}
