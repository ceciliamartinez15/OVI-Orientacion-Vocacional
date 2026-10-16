using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Logica.Servicios;
using OVI.Presentacion.Servicios;
namespace OVI.Presentacion.Controllers;
[Authorize(Roles="Estudiante,Admin")]
public class TestController(OVIDbContext db,ServicioVocacional servicio):Controller
{
 [HttpGet] public async Task<IActionResult> Index()=>View(await db.PreguntasRIASEC.Where(p=>p.Estado).OrderBy(p=>p.IdPregunta).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Index(Dictionary<int,int> respuestas)
 {
  try{var r=await servicio.CalcularAsync(UsuarioActual.Id(User),respuestas.Select(x=>new RespuestaTest(x.Key,x.Value)).ToList());return RedirectToAction(nameof(Resultado),new{id=r.Resultado.IdResultado});}
  catch(InvalidOperationException ex){ModelState.AddModelError("",ex.Message);return View(await db.PreguntasRIASEC.Where(p=>p.Estado).OrderBy(p=>p.IdPregunta).ToListAsync());}
 }
 public async Task<IActionResult> Resultado(int id)
 {
  var r=await db.Resultados.Include(x=>x.Evaluacion).Include(x=>x.Recomendaciones).ThenInclude(x=>x.Carrera).FirstOrDefaultAsync(x=>x.IdResultado==id);
  if(r==null)return NotFound();if(r.Evaluacion?.IdUsuario!=UsuarioActual.Id(User)&&!User.IsInRole("Admin"))return Forbid();return View(r);
 }
 public async Task<IActionResult> Historial()
 {
  var id=UsuarioActual.Id(User);return View(await db.Resultados.Include(x=>x.Evaluacion).Where(x=>x.Evaluacion!.IdUsuario==id).OrderByDescending(x=>x.FechaResultado).ToListAsync());
 }
}
