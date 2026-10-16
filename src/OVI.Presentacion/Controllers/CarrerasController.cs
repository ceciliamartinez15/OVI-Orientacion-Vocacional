using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using OVI.Datos;using OVI.Modelos.Entidades;
namespace OVI.Presentacion.Controllers;
public class CarrerasController(OVIDbContext db):Controller
{
 public async Task<IActionResult> Index()=>View(await db.Carreras.OrderBy(c=>c.Nombre).ToListAsync());
 [Authorize(Roles="Admin")] public IActionResult Crear()=>View(new Carrera());
 [Authorize(Roles="Admin"),HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Crear(Carrera carrera){if(!ModelState.IsValid)return View(carrera);db.Carreras.Add(carrera);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [Authorize(Roles="Admin")] public async Task<IActionResult> Editar(int id){var c=await db.Carreras.FindAsync(id);return c==null?NotFound():View(c);}
 [Authorize(Roles="Admin"),HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Editar(int id,Carrera carrera){if(id!=carrera.IdCarrera)return BadRequest();if(!ModelState.IsValid)return View(carrera);db.Update(carrera);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [Authorize(Roles="Admin"),HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Eliminar(int id){var c=await db.Carreras.FindAsync(id);if(c!=null){c.Estado=false;await db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
}
