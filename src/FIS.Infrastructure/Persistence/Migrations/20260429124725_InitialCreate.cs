using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "CLIENTE",
                schema: "dbo",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo_cliente = table.Column<string>(type: "char(1)", nullable: false),
                    codigo_cliente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre_razon_social = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    nit_ci = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fecha_registro = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTE", x => x.id_cliente);
                    table.CheckConstraint("CK_CLIENTE_tipo", "tipo_cliente IN ('N','J')");
                });

            migrationBuilder.CreateTable(
                name: "PLAN_SERVICIO",
                schema: "dbo",
                columns: table => new
                {
                    id_plan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_plan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tipo_servicio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    velocidad_bajada_mbps = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    velocidad_subida_mbps = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    precio_mensual = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLAN_SERVICIO", x => x.id_plan);
                    table.CheckConstraint("CK_PLAN_tipo_servicio", "tipo_servicio IN ('Internet Residencial','Internet Empresarial','Hosting Web','Dominio .bo','Correo Corporativo')");
                });

            migrationBuilder.CreateTable(
                name: "ROL",
                schema: "dbo",
                columns: table => new
                {
                    id_rol = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROL", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                schema: "dbo",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_rol = table.Column<byte>(type: "tinyint", nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    intentos_fallidos = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    bloqueado_hasta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_USUARIO_ROL_id_rol",
                        column: x => x.id_rol,
                        principalSchema: "dbo",
                        principalTable: "ROL",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONTRATO",
                schema: "dbo",
                columns: table => new
                {
                    id_contrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_plan = table.Column<int>(type: "int", nullable: false),
                    id_usuario_registro = table.Column<int>(type: "int", nullable: false),
                    numero_contrato = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "activo"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTRATO", x => x.id_contrato);
                    table.CheckConstraint("CK_CONTRATO_estado", "estado IN ('activo','suspendido','finalizado','cancelado')");
                    table.CheckConstraint("CK_CONTRATO_fechas", "fecha_fin > fecha_inicio");
                    table.ForeignKey(
                        name: "FK_CONTRATO_CLIENTE_id_cliente",
                        column: x => x.id_cliente,
                        principalSchema: "dbo",
                        principalTable: "CLIENTE",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONTRATO_PLAN_SERVICIO_id_plan",
                        column: x => x.id_plan,
                        principalSchema: "dbo",
                        principalTable: "PLAN_SERVICIO",
                        principalColumn: "id_plan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONTRATO_USUARIO_id_usuario_registro",
                        column: x => x.id_usuario_registro,
                        principalSchema: "dbo",
                        principalTable: "USUARIO",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MORA",
                schema: "dbo",
                columns: table => new
                {
                    id_mora = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_contrato = table.Column<int>(type: "int", nullable: false),
                    en_mora = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    fecha_inicio_mora = table.Column<DateTime>(type: "date", nullable: true),
                    monto_adeudado = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    servicio_cortado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    fecha_regularizacion = table.Column<DateTime>(type: "date", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MORA", x => x.id_mora);
                    table.ForeignKey(
                        name: "FK_MORA_CONTRATO_id_contrato",
                        column: x => x.id_contrato,
                        principalSchema: "dbo",
                        principalTable: "CONTRATO",
                        principalColumn: "id_contrato",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAGO",
                schema: "dbo",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_contrato = table.Column<int>(type: "int", nullable: false),
                    id_cajero = table.Column<int>(type: "int", nullable: false),
                    metodo_pago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    numero_recibo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    periodo_mes = table.Column<byte>(type: "tinyint", nullable: false),
                    periodo_anio = table.Column<short>(type: "smallint", nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    monto_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    monto_mora = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    conceptos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    anulado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    motivo_anulacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fecha_anulacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAGO", x => x.id_pago);
                    table.CheckConstraint("CK_PAGO_metodo", "metodo_pago IN ('Efectivo','QR','Transferencia','Tarjeta Débito','Tarjeta Crédito')");
                    table.CheckConstraint("CK_PAGO_periodo_mes", "periodo_mes BETWEEN 1 AND 12");
                    table.ForeignKey(
                        name: "FK_PAGO_CONTRATO_id_contrato",
                        column: x => x.id_contrato,
                        principalSchema: "dbo",
                        principalTable: "CONTRATO",
                        principalColumn: "id_contrato",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PAGO_USUARIO_id_cajero",
                        column: x => x.id_cajero,
                        principalSchema: "dbo",
                        principalTable: "USUARIO",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RECLAMO",
                schema: "dbo",
                columns: table => new
                {
                    id_reclamo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_contrato = table.Column<int>(type: "int", nullable: true),
                    id_tecnico = table.Column<int>(type: "int", nullable: true),
                    id_usuario_registro = table.Column<int>(type: "int", nullable: false),
                    numero_reclamo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    tipo_reclamo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, defaultValue: "Recepcionado"),
                    descripcion_problema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    solucion_aplicada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    prioridad = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)3),
                    canal_entrada = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "telefono"),
                    fecha_apertura = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    fecha_cierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    calificacion = table.Column<byte>(type: "tinyint", nullable: true),
                    tiempo_respuesta_min = table.Column<int>(type: "int", nullable: true),
                    ruta_audio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    duracion_audio_seg = table.Column<int>(type: "int", nullable: true),
                    hash_sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RECLAMO", x => x.id_reclamo);
                    table.CheckConstraint("CK_RECLAMO_calificacion", "calificacion IS NULL OR (calificacion BETWEEN 1 AND 5)");
                    table.CheckConstraint("CK_RECLAMO_canal", "canal_entrada IN ('telefono','web','presencial','app')");
                    table.CheckConstraint("CK_RECLAMO_estado", "estado IN ('Recepcionado','En Proceso','Observado','Cerrado')");
                    table.CheckConstraint("CK_RECLAMO_prioridad", "prioridad BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_RECLAMO_tipo", "tipo_reclamo IN ('Leve','Medio','Complejo')");
                    table.ForeignKey(
                        name: "FK_RECLAMO_CLIENTE_id_cliente",
                        column: x => x.id_cliente,
                        principalSchema: "dbo",
                        principalTable: "CLIENTE",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RECLAMO_CONTRATO_id_contrato",
                        column: x => x.id_contrato,
                        principalSchema: "dbo",
                        principalTable: "CONTRATO",
                        principalColumn: "id_contrato");
                    table.ForeignKey(
                        name: "FK_RECLAMO_USUARIO_id_tecnico",
                        column: x => x.id_tecnico,
                        principalSchema: "dbo",
                        principalTable: "USUARIO",
                        principalColumn: "id_usuario");
                    table.ForeignKey(
                        name: "FK_RECLAMO_USUARIO_id_usuario_registro",
                        column: x => x.id_usuario_registro,
                        principalSchema: "dbo",
                        principalTable: "USUARIO",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_codigo_cliente",
                schema: "dbo",
                table: "CLIENTE",
                column: "codigo_cliente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_email",
                schema: "dbo",
                table: "CLIENTE",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_nit",
                schema: "dbo",
                table: "CLIENTE",
                column: "nit_ci",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_nombre",
                schema: "dbo",
                table: "CLIENTE",
                column: "nombre_razon_social");

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_telefono",
                schema: "dbo",
                table: "CLIENTE",
                column: "telefono");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATO_cliente",
                schema: "dbo",
                table: "CONTRATO",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATO_id_plan",
                schema: "dbo",
                table: "CONTRATO",
                column: "id_plan");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATO_id_usuario_registro",
                schema: "dbo",
                table: "CONTRATO",
                column: "id_usuario_registro");

            migrationBuilder.CreateIndex(
                name: "IX_CONTRATO_numero_contrato",
                schema: "dbo",
                table: "CONTRATO",
                column: "numero_contrato",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MORA_id_contrato",
                schema: "dbo",
                table: "MORA",
                column: "id_contrato",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PAGO_id_cajero",
                schema: "dbo",
                table: "PAGO",
                column: "id_cajero");

            migrationBuilder.CreateIndex(
                name: "IX_PAGO_id_contrato",
                schema: "dbo",
                table: "PAGO",
                column: "id_contrato");

            migrationBuilder.CreateIndex(
                name: "IX_PAGO_numero_recibo",
                schema: "dbo",
                table: "PAGO",
                column: "numero_recibo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RECLAMO_id_cliente",
                schema: "dbo",
                table: "RECLAMO",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_RECLAMO_id_contrato",
                schema: "dbo",
                table: "RECLAMO",
                column: "id_contrato");

            migrationBuilder.CreateIndex(
                name: "IX_RECLAMO_id_tecnico",
                schema: "dbo",
                table: "RECLAMO",
                column: "id_tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_RECLAMO_id_usuario_registro",
                schema: "dbo",
                table: "RECLAMO",
                column: "id_usuario_registro");

            migrationBuilder.CreateIndex(
                name: "IX_RECLAMO_numero_reclamo",
                schema: "dbo",
                table: "RECLAMO",
                column: "numero_reclamo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ROL_nombre_rol",
                schema: "dbo",
                table: "ROL",
                column: "nombre_rol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_email",
                schema: "dbo",
                table: "USUARIO",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_id_rol",
                schema: "dbo",
                table: "USUARIO",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_username",
                schema: "dbo",
                table: "USUARIO",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MORA",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PAGO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RECLAMO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CONTRATO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CLIENTE",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PLAN_SERVICIO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "USUARIO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ROL",
                schema: "dbo");
        }
    }
}
