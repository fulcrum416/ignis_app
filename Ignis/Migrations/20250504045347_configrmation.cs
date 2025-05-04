using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ignis.Migrations
{
    /// <inheritdoc />
    public partial class configrmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "confirmationcode",
                table: "invites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "firstname",
                table: "invites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lastname",
                table: "invites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tempcode",
                table: "invites",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confirmationcode",
                table: "invites");

            migrationBuilder.DropColumn(
                name: "firstname",
                table: "invites");

            migrationBuilder.DropColumn(
                name: "lastname",
                table: "invites");

            migrationBuilder.DropColumn(
                name: "tempcode",
                table: "invites");
        }
    }
}
