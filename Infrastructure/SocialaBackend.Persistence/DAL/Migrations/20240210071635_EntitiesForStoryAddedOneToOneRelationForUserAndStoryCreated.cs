using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class EntitiesForStoryAddedOneToOneRelationForUserAndStoryCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Story",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Story", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Story_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WatchCount = table.Column<int>(type: "int", nullable: false),
                    StoryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryItem_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryItemWatcher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoryItemId = table.Column<int>(type: "int", nullable: false),
                    WatcherId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryItemWatcher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryItemWatcher_AspNetUsers_WatcherId",
                        column: x => x.WatcherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryItemWatcher_StoryItem_StoryItemId",
                        column: x => x.StoryItemId,
                        principalTable: "StoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Story_AppUserId",
                table: "Story",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryItem_StoryId",
                table: "StoryItem",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemWatcher_StoryItemId",
                table: "StoryItemWatcher",
                column: "StoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemWatcher_WatcherId",
                table: "StoryItemWatcher",
                column: "WatcherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryItemWatcher");

            migrationBuilder.DropTable(
                name: "StoryItem");

            migrationBuilder.DropTable(
                name: "Story");
        }
    }
}
