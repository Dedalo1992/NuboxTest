using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nubox.BridgeApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResumenesAsistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpresaId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrabajadorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodoClave = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Rut = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    HorasTrabajadas = table.Column<int>(type: "int", nullable: false),
                    HorasExtras = table.Column<int>(type: "int", nullable: false),
                    Ausencias = table.Column<int>(type: "int", nullable: false),
                    AusenciaLicenciaMedica = table.Column<int>(type: "int", nullable: false),
                    TotalHorasRemunerables = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalculatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumenesAsistencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sincronizaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpresaId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodoClave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InicioUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sincronizaciones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesAsistencia_EmpresaId_TrabajadorId_PeriodoClave",
                table: "ResumenesAsistencia",
                columns: new[] { "EmpresaId", "TrabajadorId", "PeriodoClave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sincronizaciones_CorrelationId",
                table: "Sincronizaciones",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sincronizaciones_EmpresaId_PeriodoClave",
                table: "Sincronizaciones",
                columns: new[] { "EmpresaId", "PeriodoClave" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResumenesAsistencia");

            migrationBuilder.DropTable(
                name: "Sincronizaciones");
        }
    }
}
