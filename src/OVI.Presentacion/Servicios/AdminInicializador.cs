using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
namespace OVI.Presentacion.Servicios;
public static class AdminInicializador
{
 public static async Task CrearSiConfiguradoAsync(IServiceProvider services,IConfiguration config)
 {
  var email=config["OVI_ADMIN_EMAIL"];var password=config["OVI_ADMIN_PASSWORD"];
  if(string.IsNullOrWhiteSpace(email)||string.IsNullOrWhiteSpace(password))return;
  if(password.Length<12)throw new InvalidOperationException("OVI_ADMIN_PASSWORD debe tener al menos 12 caracteres.");
  using var scope=services.CreateScope();var db=scope.ServiceProvider.GetRequiredService<OVIDbContext>();
  var hasher=scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
  email=email.Trim().ToLowerInvariant();
  var existing=await db.Usuarios.FirstOrDefaultAsync(u=>u.Correo==email);
  if(existing!=null)return; // Nunca elevar privilegios de una cuenta existente automáticamente.
  var admin=new Usuario{IdRol=3,IdInstitucion=1,Nombre="Administrador",Apellido="VocalO",Correo=email,Estado=true};
  admin.PasswordHash=hasher.HashPassword(admin,password);db.Usuarios.Add(admin);await db.SaveChangesAsync();
 }
}
