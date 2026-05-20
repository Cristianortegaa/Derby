using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Derby.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPartidoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos");

            migrationBuilder.AlterColumn<int>(
                name: "CompeticionId",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Jornada",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos",
                column: "CompeticionId",
                principalTable: "Competiciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Jornada",
                table: "Partidos");

            migrationBuilder.AlterColumn<int>(
                name: "CompeticionId",
                table: "Partidos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos",
                column: "CompeticionId",
                principalTable: "Competiciones",
                principalColumn: "Id");
        }
    }
}
