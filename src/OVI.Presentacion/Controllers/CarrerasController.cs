using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
using OVI.Presentacion.Models;

namespace OVI.Presentacion.Controllers;

// CRUD de carreras del catálogo
[Authorize(Roles = "Admin")]
public class CarrerasController(OVIDbContext db) : Controller
{
    public async Task<IActionResult> Index()
        => View(await db.Carreras.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync());

    [HttpGet]
    public IActionResult Crear() => View("Formulario", new CarreraFormVM());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CarreraFormVM vm)
    {
        if (!await ValidarAsync(vm)) return View("Formulario", vm);

        var carrera = new Carrera();
        Aplicar(carrera, vm);
        db.Carreras.Add(carrera);
        await db.SaveChangesAsync();
        TempData["Ok"] = "Carrera creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var c = await db.Carreras.AsNoTracking().FirstOrDefaultAsync(x => x.IdCarrera == id);
        if (c == null) return NotFound();
        return View("Formulario", new CarreraFormVM
        {
            IdCarrera = c.IdCarrera,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Tipo = c.Tipo,
            Institucion = c.Institucion,
            Estado = c.Estado,
            Areas = (c.AreasRIASEC ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, CarreraFormVM vm)
    {
        if (id != vm.IdCarrera) return BadRequest();
        var carrera = await db.Carreras.FindAsync(id);
        if (carrera == null) return NotFound();
        if (!await ValidarAsync(vm)) return View("Formulario", vm);

        Aplicar(carrera, vm);
        await db.SaveChangesAsync();
        TempData["Ok"] = "Carrera actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // Activa o desactiva una carrera (una carrera inactiva deja de recomendarse en nuevos tests).
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Estado(int id)
    {
        var c = await db.Carreras.FindAsync(id);
        if (c == null) return NotFound();
        c.Estado = !c.Estado;
        await db.SaveChangesAsync();
        TempData["Ok"] = c.Estado ? "Carrera activada." : "Carrera desactivada.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ValidarAsync(CarreraFormVM vm)
    {
        vm.Nombre = (vm.Nombre ?? "").Trim();
        vm.Areas = vm.Areas.Where(a => CarreraFormVM.AreasDisponibles.Contains(a)).Distinct().ToList();

        if (!CarreraFormVM.Tipos.Contains(vm.Tipo))
            ModelState.AddModelError(nameof(vm.Tipo), "Selecciona un tipo válido.");
        if (vm.Areas.Count == 0)
            ModelState.AddModelError(nameof(vm.Areas), "Selecciona al menos un área RIASEC; con ella se calcula la afinidad.");
        if (vm.Nombre.Length > 0 && await db.Carreras.AnyAsync(x => x.Nombre == vm.Nombre && x.IdCarrera != vm.IdCarrera))
            ModelState.AddModelError(nameof(vm.Nombre), "Ya existe una carrera con ese nombre.");

        return ModelState.IsValid;
    }

    private static void Aplicar(Carrera c, CarreraFormVM vm)
    {
        c.Nombre = vm.Nombre;
        c.Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim();
        c.Tipo = vm.Tipo;
        c.Institucion = string.IsNullOrWhiteSpace(vm.Institucion) ? null : vm.Institucion.Trim();
        c.AreasRIASEC = string.Join(",", vm.Areas);
        c.Estado = vm.Estado;
    }
}
