using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
using OVI.Presentacion.Models;
using OVI.Presentacion.Servicios;

namespace OVI.Presentacion.Controllers;

// CRUD de usuarios para el administrador
[Authorize(Roles = "Admin")]
public class UsuariosController(OVIDbContext db, IPasswordHasher<Usuario> hasher) : Controller
{
    private const int MinContrasena = 8;

    public async Task<IActionResult> Index()
        => View(await db.Usuarios.AsNoTracking().Include(x => x.Rol).Include(x => x.Institucion).OrderBy(x => x.IdUsuario).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> Crear() => View("Formulario", await CargarListasAsync(new UsuarioFormVM()));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(UsuarioFormVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Contrasena))
            ModelState.AddModelError(nameof(vm.Contrasena), "La contraseña es obligatoria.");
        if (!await ValidarAsync(vm)) return View("Formulario", await CargarListasAsync(vm));

        var u = new Usuario { Estado = vm.Estado };
        Aplicar(u, vm);
        u.PasswordHash = hasher.HashPassword(u, vm.Contrasena!);
        db.Usuarios.Add(u);
        await db.SaveChangesAsync();
        TempData["Ok"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var u = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.IdUsuario == id);
        if (u == null) return NotFound();
        return View("Formulario", await CargarListasAsync(new UsuarioFormVM
        {
            IdUsuario = u.IdUsuario,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Correo = u.Correo,
            IdRol = u.IdRol,
            IdInstitucion = u.IdInstitucion,
            Estado = u.Estado
        }));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, UsuarioFormVM vm)
    {
        if (id != vm.IdUsuario) return BadRequest();
        var u = await db.Usuarios.FindAsync(id);
        if (u == null) return NotFound();

        // Evita que el administrador se quite el acceso a sí mismo.
        if (id == UsuarioActual.Id(User))
        {
            if (!vm.Estado) ModelState.AddModelError(nameof(vm.Estado), "No puedes desactivar tu propia cuenta.");
            if (vm.IdRol != u.IdRol) ModelState.AddModelError(nameof(vm.IdRol), "No puedes cambiar tu propio rol.");
        }
        if (!await ValidarAsync(vm)) return View("Formulario", await CargarListasAsync(vm));

        Aplicar(u, vm);
        u.Estado = vm.Estado;
        if (!string.IsNullOrWhiteSpace(vm.Contrasena))
            u.PasswordHash = hasher.HashPassword(u, vm.Contrasena);
        await db.SaveChangesAsync();
        TempData["Ok"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Estado(int id)
    {
        if (id == UsuarioActual.Id(User)) return BadRequest("No puedes desactivar tu propia cuenta.");
        var u = await db.Usuarios.FindAsync(id);
        if (u == null) return NotFound();
        u.Estado = !u.Estado;
        await db.SaveChangesAsync();
        TempData["Ok"] = u.Estado ? "Usuario activado." : "Usuario desactivado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ValidarAsync(UsuarioFormVM vm)
    {
        vm.Nombre = (vm.Nombre ?? "").Trim();
        vm.Apellido = (vm.Apellido ?? "").Trim();
        vm.Correo = (vm.Correo ?? "").Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(vm.Contrasena) && vm.Contrasena.Length < MinContrasena)
            ModelState.AddModelError(nameof(vm.Contrasena), $"La contraseña debe tener al menos {MinContrasena} caracteres.");
        if (!await db.Roles.AnyAsync(r => r.IdRol == vm.IdRol))
            ModelState.AddModelError(nameof(vm.IdRol), "Selecciona un rol válido.");
        if (!await db.Instituciones.AnyAsync(i => i.IdInstitucion == vm.IdInstitucion))
            ModelState.AddModelError(nameof(vm.IdInstitucion), "Selecciona una institución válida.");
        if (vm.Correo.Length > 0 && await db.Usuarios.AnyAsync(x => x.Correo == vm.Correo && x.IdUsuario != vm.IdUsuario))
            ModelState.AddModelError(nameof(vm.Correo), "Este correo ya está registrado.");

        return ModelState.IsValid;
    }

    private static void Aplicar(Usuario u, UsuarioFormVM vm)
    {
        u.Nombre = vm.Nombre;
        u.Apellido = vm.Apellido;
        u.Correo = vm.Correo;
        u.IdRol = vm.IdRol;
        u.IdInstitucion = vm.IdInstitucion;
    }

    private async Task<UsuarioFormVM> CargarListasAsync(UsuarioFormVM vm)
    {
        vm.Roles = await db.Roles.AsNoTracking().OrderBy(r => r.IdRol)
            .Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString())).ToListAsync();
        // Se incluyen las inactivas solo si son la actual del usuario, para no perder el valor al editar.
        vm.Instituciones = await db.Instituciones.AsNoTracking()
            .Where(i => i.Estado || i.IdInstitucion == vm.IdInstitucion).OrderBy(i => i.Nombre)
            .Select(i => new SelectListItem(i.Nombre, i.IdInstitucion.ToString())).ToListAsync();
        return vm;
    }
}
