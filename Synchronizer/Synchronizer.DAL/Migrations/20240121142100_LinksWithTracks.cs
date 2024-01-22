using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synchronizer.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class LinksWithTracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncTracks_Links_SynchronizedPlaylistLinkId",
                table: "SyncTracks");

            migrationBuilder.DropIndex(
                name: "IX_SyncTracks_SynchronizedPlaylistLinkId",
                table: "SyncTracks");

            migrationBuilder.DropColumn(
                name: "SynchronizedPlaylistLinkId",
                table: "SyncTracks");

            migrationBuilder.AddColumn<Guid>(
                name: "LinkId",
                table: "SyncTracks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SyncTracks_LinkId",
                table: "SyncTracks",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncTracks_Links_LinkId",
                table: "SyncTracks",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncTracks_Links_LinkId",
                table: "SyncTracks");

            migrationBuilder.DropIndex(
                name: "IX_SyncTracks_LinkId",
                table: "SyncTracks");

            migrationBuilder.DropColumn(
                name: "LinkId",
                table: "SyncTracks");

            migrationBuilder.AddColumn<Guid>(
                name: "SynchronizedPlaylistLinkId",
                table: "SyncTracks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncTracks_SynchronizedPlaylistLinkId",
                table: "SyncTracks",
                column: "SynchronizedPlaylistLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncTracks_Links_SynchronizedPlaylistLinkId",
                table: "SyncTracks",
                column: "SynchronizedPlaylistLinkId",
                principalTable: "Links",
                principalColumn: "Id");
        }
    }
}
