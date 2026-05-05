using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppWeb.Migrations
{
    /// <inheritdoc />
    public partial class CambiosBD_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Comentado porque la columna ya existe en la base de datos
            /*
            migrationBuilder.AddColumn<int>(
                name: "IdRol",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
            */

            migrationBuilder.AlterColumn<int>(
                name: "VideoJuegosId",
                table: "Detalle_Compra",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Comentado porque la tabla ya existe en la base de datos
            /*
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });
            */

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Compra_VideoJuegosId",
                table: "Detalle_Compra",
                column: "VideoJuegosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Compra_Videojuegos_VideoJuegosId",
                table: "Detalle_Compra",
                column: "VideoJuegosId",
                principalTable: "Videojuegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "Roles",
                principalColumn: "IdRol",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Compra_Videojuegos_VideoJuegosId",
                table: "Detalle_Compra");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Detalle_Compra_VideoJuegosId",
                table: "Detalle_Compra");

            migrationBuilder.DropColumn(
                name: "IdRol",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "VideoJuegosId",
                table: "Detalle_Compra",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}