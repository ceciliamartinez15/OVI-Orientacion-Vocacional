using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
namespace OVI.Presentacion.Controllers;
[Authorize(Roles="Admin")]
public class PreguntasController(OVIDbContext db):Controller
{
 public async Task<IActionResult> Index()=>View(await db.PreguntasRIASEC.OrderBy(x=>x.IdPregunta).ToListAsync());
 public IActionResult Crear()=>View("Formulario",new PreguntaRIASEC());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Crear(PreguntaRIASEC p){if(!ModelState.IsValid)return View("Formulario",p);db.PreguntasRIASEC.Add(p);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 public async Task<IActionResult> Editar(int id){var p=await db.PreguntasRIASEC.FindAsync(id);return p==null?NotFound():View("Formulario",p);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Editar(int id,PreguntaRIASEC p){if(id!=p.IdPregunta)return BadRequest();if(!ModelState.IsValid)return View("Formulario",p);var original=await db.PreguntasRIASEC.FindAsync(id);if(original==null)return NotFound();original.Enunciado=p.Enunciado;original.CategoriaRIASEC=p.CategoriaRIASEC;original.Estado=p.Estado;original.Valor=p.Valor;await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Eliminar(int id){var p=await db.PreguntasRIASEC.FindAsync(id);if(p!=null){p.Estado=false;await db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
}
