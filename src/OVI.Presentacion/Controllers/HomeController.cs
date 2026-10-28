using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Presentacion.Servicios;

namespace OVI.Presentacion.Controllers;

public class HomeController(OVIDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Orientador"))
            {
                ViewBag.Estudiantes = await db.Usuarios.CountAsync(u => u.Estado && u.Rol != null && u.Rol.Nombre == "Estudiante");
                ViewBag.Evaluaciones = await db.Evaluaciones.CountAsync(e => e.Estado == "Finalizada");
            }
            else
            {
                int id = UsuarioActual.Id(User);
                ViewBag.MisEvaluaciones = await db.Evaluaciones.CountAsync(e => e.IdUsuario == id && e.Estado == "Finalizada");
                var ultimo = await db.Resultados.AsNoTracking()
                    .Where(r => r.Evaluacion != null && r.Evaluacion.IdUsuario == id)
                    .OrderByDescending(r => r.FechaResultado)
                    .Select(r => new { r.IdResultado, r.PerfilDominante })
                    .FirstOrDefaultAsync();
                ViewBag.UltimoPerfil = ultimo?.PerfilDominante;
                ViewBag.UltimoResultadoId = ultimo?.IdResultado;
            }
        }
        return View();
    }

    public IActionResult Privacy() => View();
    public IActionResult Error() => View("Error");
}
