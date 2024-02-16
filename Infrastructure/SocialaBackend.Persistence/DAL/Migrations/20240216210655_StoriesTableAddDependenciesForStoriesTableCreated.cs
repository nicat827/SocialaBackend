using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class StoriesTableAddDependenciesForStoriesTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryItems",
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
                    table.PrimaryKey("PK_StoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryItems_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryItemWatchers",
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
                    table.PrimaryKey("PK_StoryItemWatchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryItemWatchers_AspNetUsers_WatcherId",
                        column: x => x.WatcherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoryItemWatchers_StoryItems_StoryItemId",
                        column: x => x.StoryItemId,
                        principalTable: "StoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_OwnerId",
                table: "Stories",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryItems_StoryId",
                table: "StoryItems",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemWatchers_StoryItemId",
                table: "StoryItemWatchers",
                column: "StoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryItemWatchers_WatcherId",
                table: "StoryItemWatchers",
                column: "WatcherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryItemWatchers");

            migrationBuilder.DropTable(
                name: "StoryItems");

            migrationBuilder.DropTable(
                name: "Stories");
        }
    }
}
