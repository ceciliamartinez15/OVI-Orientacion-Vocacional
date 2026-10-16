using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Logica.Servicios;
using OVI.Presentacion.Servicios;
namespace OVI.Presentacion.Controllers;
[Authorize]
public class ChatController(ServicioChatbot bot,ServicioIA ia,OVIDbContext db):Controller
{
 [HttpGet] public IActionResult Index(){ViewBag.IAConfigurada=ia.Configurada;return View();}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Index(string mensaje,CancellationToken ct)
 {
  ViewBag.IAConfigurada=ia.Configurada;ViewBag.Pregunta=mensaje;
  if(string.IsNullOrWhiteSpace(mensaje)||mensaje.Length>500){ModelState.AddModelError("","Escribe una pregunta de máximo 500 caracteres.");return View();}
  var id=UsuarioActual.Id(User);var perfil=await db.Resultados.Where(x=>x.Evaluacion!.IdUsuario==id).OrderByDescending(x=>x.FechaResultado).Select(x=>x.PerfilDominante).FirstOrDefaultAsync(ct);
  var respuesta=await ia.PreguntarAsync(mensaje,perfil,ct);
  ViewBag.EsIA=respuesta!=null;ViewBag.Respuesta=respuesta??await bot.ResponderAsync(mensaje);
  return View();
 }
}
