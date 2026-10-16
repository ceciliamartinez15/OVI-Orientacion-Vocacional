using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
namespace OVI.Presentacion.Controllers;
[Authorize(Roles="Admin")]
public class InstitucionesController(OVIDbContext db):Controller
{
 public async Task<IActionResult> Index()=>View(await db.Instituciones.OrderBy(x=>x.Nombre).ToListAsync());
 public IActionResult Crear()=>View("Formulario",new Institucion());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Crear(Institucion x){if(!ModelState.IsValid)return View("Formulario",x);db.Instituciones.Add(x);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 public async Task<IActionResult> Editar(int id){var x=await db.Instituciones.FindAsync(id);return x==null?NotFound():View("Formulario",x);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Editar(int id,Institucion x){if(id!=x.IdInstitucion)return BadRequest();if(!ModelState.IsValid)return View("Formulario",x);var old=await db.Instituciones.FindAsync(id);if(old==null)return NotFound();old.Nombre=x.Nombre;old.Direccion=x.Direccion;old.Telefono=x.Telefono;old.Estado=x.Estado;await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Eliminar(int id){var x=await db.Instituciones.FindAsync(id);if(x!=null){x.Estado=false;await db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
}
