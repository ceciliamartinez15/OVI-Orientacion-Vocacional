using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace OVI.Datos.Migraciones;

// Agrega a Carreras las áreas RIASEC (separadas por coma) con las que se calcula la afinidad.
// Las carreras existentes quedan en NULL y siguen usando la regla por nombre.
[DbContext(typeof(OVIDbContext))]
[Migration("20261031180000_AgregarAreasCarrera")]
public partial class AgregarAreasCarrera : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<string>(name: "AreasRIASEC", table: "Carreras", type: "nvarchar(100)", maxLength: 100, nullable: true);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "AreasRIASEC", table: "Carreras");
}
