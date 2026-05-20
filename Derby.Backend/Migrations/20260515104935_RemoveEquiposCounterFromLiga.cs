using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Derby.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEquiposCounterFromLiga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_Partidos_CompeticionId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "CompeticionId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Equipos",
                table: "Ligas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompeticionId",
                table: "Partidos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Equipos",
                table: "Ligas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_CompeticionId",
                table: "Partidos",
                column: "CompeticionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos",
                column: "CompeticionId",
                principalTable: "Competiciones",
                principalColumn: "Id");
        }
    }
}
