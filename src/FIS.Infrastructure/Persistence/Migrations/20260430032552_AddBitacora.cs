using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBitacora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BITACORA",
                schema: "dbo",
                columns: table => new
                {
                    id_bitacora = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tabla = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    operacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    valores_anteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valores_nuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usuario_accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fecha_hora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BITACORA", x => x.id_bitacora);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BITACORA",
                schema: "dbo");
        }
    }
}
