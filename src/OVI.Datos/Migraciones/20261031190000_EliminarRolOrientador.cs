using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace OVI.Datos.Migraciones;

// Elimina el rol opcional "Orientador". Si algún usuario lo tenía, pasa a Estudiante (sin privilegios de gestión).
[DbContext(typeof(OVIDbContext))]
[Migration("20261031190000_EliminarRolOrientador")]
public partial class EliminarRolOrientador : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE Usuarios SET IdRol = 1 WHERE IdRol = 2");
        migrationBuilder.Sql("DELETE FROM Roles WHERE IdRol = 2");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM Roles WHERE IdRol = 2) INSERT INTO Roles (IdRol, Descripcion, Nombre) VALUES (2, N'Consulta estadísticas', N'Orientador')");
}