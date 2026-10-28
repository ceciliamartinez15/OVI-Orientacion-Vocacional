using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using OVI.Datos;
namespace OVI.Presentacion.Controllers;
[Authorize(Roles="Admin,Orientador")]
public class DashboardController(OVIDbContext db):Controller
{
 public async Task<IActionResult> Index(){ViewBag.Total=await db.Resultados.CountAsync();ViewBag.Carreras=await db.Carreras.CountAsync(c=>c.Estado);ViewBag.Estudiantes=await db.Usuarios.CountAsync(u=>u.Estado);var datos=await db.Resultados.GroupBy(r=>r.PerfilDominante).Select(g=>new PerfilVM{Perfil=g.Key,Cantidad=g.Count()}).OrderByDescending(x=>x.Cantidad).ToListAsync();return View(datos);}
 public class PerfilVM{public string Perfil{get;set;}="";public int Cantidad{get;set;}}
}
