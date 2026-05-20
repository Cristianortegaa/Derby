using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Derby.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDivisionFromEquipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "Equipos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Equipos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
