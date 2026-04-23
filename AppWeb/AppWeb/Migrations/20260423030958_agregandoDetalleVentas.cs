using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppWeb.Migrations
{
    /// <inheritdoc />
    public partial class agregandoDetalleVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_videojuegos_categorias",
                table: "Videojuegos");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Videojuegos_VideojuegoId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_VideojuegoId",
                table: "Compras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categorias",
                table: "categorias");

            migrationBuilder.DropColumn(
                name: "VideojuegoId",
                table: "Compras");

            migrationBuilder.RenameTable(
                name: "categorias",
                newName: "Categorias");

            migrationBuilder.AlterColumn<int>(
                name: "idcategoria",
                table: "Videojuegos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "idcategoria");

            migrationBuilder.CreateTable(
                name: "Detalle_Compra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoJuegosId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoCompra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaHoraTransaccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodigoTransaccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdCompra = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Compra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Detalle_Compra_Compras_IdCompra",
                        column: x => x.IdCompra,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videojuegos_idcategoria",
                table: "Videojuegos",
                column: "idcategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Compra_IdCompra",
                table: "Detalle_Compra",
                column: "IdCompra");

            migrationBuilder.AddForeignKey(
                name: "FK_Videojuegos_Categorias_idcategoria",
                table: "Videojuegos",
                column: "idcategoria",
                principalTable: "Categorias",
                principalColumn: "idcategoria",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videojuegos_Categorias_idcategoria",
                table: "Videojuegos");

            migrationBuilder.DropTable(
                name: "Detalle_Compra");

            migrationBuilder.DropIndex(
                name: "IX_Videojuegos_idcategoria",
                table: "Videojuegos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "categorias");

            migrationBuilder.AlterColumn<string>(
                name: "idcategoria",
                table: "Videojuegos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VideojuegoId",
                table: "Compras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_categorias",
                table: "categorias",
                column: "idcategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_VideojuegoId",
                table: "Compras",
                column: "VideojuegoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Videojuegos_VideojuegoId",
                table: "Compras",
                column: "VideojuegoId",
                principalTable: "Videojuegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
