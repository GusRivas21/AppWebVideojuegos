using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppWeb.Migrations
{
    /// <inheritdoc />
    public partial class tabla_de_categorias_separada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Categoria",
                table: "Videojuegos",
                newName: "idcategoria");

            migrationBuilder.RenameColumn(
                name: "Salt",
                table: "Usuarios",
                newName: "salt");

            migrationBuilder.AlterColumn<bool>(
                name: "Promocion",
                table: "Videojuegos",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    idcategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.idcategoria);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.RenameColumn(
                name: "idcategoria",
                table: "Videojuegos",
                newName: "Categoria");

            migrationBuilder.RenameColumn(
                name: "salt",
                table: "Usuarios",
                newName: "Salt");

            migrationBuilder.AlterColumn<bool>(
                name: "Promocion",
                table: "Videojuegos",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }
    }
}
