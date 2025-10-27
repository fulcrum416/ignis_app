using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ignis.Migrations
{
    /// <inheritdoc />
    public partial class invitate_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "nolimit",
                table: "invites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "totaldays",
                table: "invites",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nolimit",
                table: "invites");

            migrationBuilder.DropColumn(
                name: "totaldays",
                table: "invites");
        }
    }
}
