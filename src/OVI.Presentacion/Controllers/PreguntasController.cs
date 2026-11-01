using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
namespace OVI.Presentacion.Controllers;
[Authorize(Roles="Admin")]
public class PreguntasController(OVIDbContext db):Controller
{
 private static readonly string[] Categorias=["Realista","Investigador","Artistico","Social","Emprendedor","Convencional"];
 private void ValidarCategoria(PreguntaRIASEC p){p.Enunciado=(p.Enunciado??"").Trim();if(!Categorias.Contains(p.CategoriaRIASEC))ModelState.AddModelError(nameof(p.CategoriaRIASEC),"Selecciona un área RIASEC válida.");}
 public async Task<IActionResult> Index()=>View(await db.PreguntasRIASEC.OrderBy(x=>x.IdPregunta).ToListAsync());
 public IActionResult Crear()=>View("Formulario",new PreguntaRIASEC());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Crear(PreguntaRIASEC p){ValidarCategoria(p);if(!ModelState.IsValid)return View("Formulario",p);db.PreguntasRIASEC.Add(p);await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 public async Task<IActionResult> Editar(int id){var p=await db.PreguntasRIASEC.FindAsync(id);return p==null?NotFound():View("Formulario",p);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Editar(int id,PreguntaRIASEC p){if(id!=p.IdPregunta)return BadRequest();ValidarCategoria(p);if(!ModelState.IsValid)return View("Formulario",p);var original=await db.PreguntasRIASEC.FindAsync(id);if(original==null)return NotFound();original.Enunciado=p.Enunciado;original.CategoriaRIASEC=p.CategoriaRIASEC;original.Estado=p.Estado;original.Valor=p.Valor;await db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
 // Activa o desactiva las respuestas 
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Estado(int id){var p=await db.PreguntasRIASEC.FindAsync(id);if(p==null)return NotFound();p.Estado=!p.Estado;await db.SaveChangesAsync();TempData["Ok"]=p.Estado?"Pregunta activada.":"Pregunta desactivada.";return RedirectToAction(nameof(Index));}
}
