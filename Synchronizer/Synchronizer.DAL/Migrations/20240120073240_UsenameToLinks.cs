using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synchronizer.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class UsenameToLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Links",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Links");
        }
    }
}
