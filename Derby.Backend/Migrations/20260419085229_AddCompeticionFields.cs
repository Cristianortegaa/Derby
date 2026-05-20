using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Derby.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCompeticionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Grupo",
                table: "Competiciones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoJuego",
                table: "Competiciones",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grupo",
                table: "Competiciones");

            migrationBuilder.DropColumn(
                name: "TipoJuego",
                table: "Competiciones");
        }
    }
}
