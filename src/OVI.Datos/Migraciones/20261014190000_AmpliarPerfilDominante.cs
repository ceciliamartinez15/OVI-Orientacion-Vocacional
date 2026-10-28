using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace OVI.Datos.Migraciones;
[DbContext(typeof(OVIDbContext))]
[Migration("20261014190000_AmpliarPerfilDominante")]
public partial class AmpliarPerfilDominante : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<string>(name:"PerfilDominante",table:"Resultados",type:"nvarchar(255)",maxLength:255,nullable:false,oldClrType:typeof(string),oldType:"nvarchar(10)",oldMaxLength:10);
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<string>(name:"PerfilDominante",table:"Resultados",type:"nvarchar(10)",maxLength:10,nullable:false,oldClrType:typeof(string),oldType:"nvarchar(255)",oldMaxLength:255);
}
