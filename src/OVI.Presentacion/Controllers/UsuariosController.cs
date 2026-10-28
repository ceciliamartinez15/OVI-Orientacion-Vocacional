using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Presentacion.Servicios;
namespace OVI.Presentacion.Controllers;
[Authorize(Roles="Admin")]
public class UsuariosController(OVIDbContext db):Controller
{
 public async Task<IActionResult> Index()=>View(await db.Usuarios.Include(x=>x.Rol).OrderBy(x=>x.IdUsuario).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Estado(int id)
 {
  if(id==UsuarioActual.Id(User))return BadRequest("No puedes desactivar tu propia cuenta.");
  var u=await db.Usuarios.FindAsync(id);if(u==null)return NotFound();u.Estado=!u.Estado;await db.SaveChangesAsync();return RedirectToAction(nameof(Index));
 }
}
