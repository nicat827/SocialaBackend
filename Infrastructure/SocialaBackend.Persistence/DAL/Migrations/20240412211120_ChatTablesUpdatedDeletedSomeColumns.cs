using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialaBackend.Persistence.DAL.Migrations
{
    public partial class ChatTablesUpdatedDeletedSomeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageIsChecked",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageIsMedia",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageSendedAt",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageSendedBy",
                table: "Chats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LastMessageIsChecked",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LastMessageIsMedia",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageSendedAt",
                table: "Chats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMessageSendedBy",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
