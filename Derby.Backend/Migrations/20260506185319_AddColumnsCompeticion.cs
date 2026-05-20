using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Derby.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsCompeticion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Competiciones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Competiciones",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Competiciones");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Competiciones");
        }
    }
}
