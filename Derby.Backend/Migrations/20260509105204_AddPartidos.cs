using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Derby.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPartidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Finalizado",
                table: "Partidos");

            migrationBuilder.RenameColumn(
                name: "GolesVisitantes",
                table: "Partidos",
                newName: "GolesVisitante");

            migrationBuilder.AlterColumn<int>(
                name: "CompeticionId",
                table: "Partidos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Partidos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHora",
                table: "Partidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LigaId",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_LigaId",
                table: "Partidos",
                column: "LigaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos",
                column: "CompeticionId",
                principalTable: "Competiciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Ligas_LigaId",
                table: "Partidos",
                column: "LigaId",
                principalTable: "Ligas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Ligas_LigaId",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_Partidos_LigaId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "FechaHora",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "LigaId",
                table: "Partidos");

            migrationBuilder.RenameColumn(
                name: "GolesVisitante",
                table: "Partidos",
                newName: "GolesVisitantes");

            migrationBuilder.AlterColumn<int>(
                name: "CompeticionId",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Partidos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Finalizado",
                table: "Partidos",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Competiciones_CompeticionId",
                table: "Partidos",
                column: "CompeticionId",
                principalTable: "Competiciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
