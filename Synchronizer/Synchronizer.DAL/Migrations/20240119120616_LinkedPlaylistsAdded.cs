using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synchronizer.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class LinkedPlaylistsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    YandexPlaylistId = table.Column<long>(type: "bigint", nullable: false),
                    VkPlaylistId = table.Column<long>(type: "bigint", nullable: false),
                    MainMusicService = table.Column<string>(type: "text", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Links");
        }
    }
}
