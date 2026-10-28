using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
using OVI.Presentacion.Models;
namespace OVI.Presentacion.Controllers;
public class CuentaController(OVIDbContext db,IPasswordHasher<Usuario> hasher):Controller
{
 [AllowAnonymous,HttpGet] public IActionResult Registro()=>View(new RegistroVM());
 [AllowAnonymous,HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Registro(RegistroVM vm)
 {
  if(!ModelState.IsValid)return View(vm);
  var correo=vm.Correo.Trim().ToLowerInvariant();
  if(await db.Usuarios.AnyAsync(x=>x.Correo==correo)){ModelState.AddModelError(nameof(vm.Correo),"Este correo ya está registrado.");return View(vm);}
  var u=new Usuario{Nombre=vm.Nombre.Trim(),Apellido=vm.Apellido.Trim(),Correo=correo,IdRol=1,IdInstitucion=1,Estado=true};
  u.PasswordHash=hasher.HashPassword(u,vm.Contrasena);
  db.Usuarios.Add(u);await db.SaveChangesAsync();await Ingresar(u,"Estudiante");return RedirectToAction("Index","Home");
 }
 [AllowAnonymous,HttpGet] public IActionResult Login(string? returnUrl=null){ViewBag.ReturnUrl=Url.IsLocalUrl(returnUrl)?returnUrl:null;return View(new LoginVM());}
 [AllowAnonymous,HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginVM vm,string? returnUrl=null)
 {
  if(!ModelState.IsValid)return View(vm);
  var u=await db.Usuarios.Include(x=>x.Rol).FirstOrDefaultAsync(x=>x.Correo==vm.Correo.Trim().ToLower() && x.Estado);
  if(u==null || string.IsNullOrWhiteSpace(u.PasswordHash) || hasher.VerifyHashedPassword(u,u.PasswordHash,vm.Contrasena)==PasswordVerificationResult.Failed)
  {ModelState.AddModelError("","Correo o contraseña incorrectos.");return View(vm);}
  await Ingresar(u,u.Rol?.Nombre??"Estudiante");return Url.IsLocalUrl(returnUrl)?LocalRedirect(returnUrl!):RedirectToAction("Index","Home");
 }
 [Authorize,HttpGet] public IActionResult Denegado()=>View();
 [Authorize,HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Salir(){await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);return RedirectToAction("Index","Home");}
 private async Task Ingresar(Usuario u,string rol)
 {
  var claims=new[]{new Claim(ClaimTypes.NameIdentifier,u.IdUsuario.ToString()),new Claim(ClaimTypes.Name,u.Nombre+" "+u.Apellido),new Claim(ClaimTypes.Email,u.Correo),new Claim(ClaimTypes.Role,rol)};
  await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)),new AuthenticationProperties{IsPersistent=false,ExpiresUtc=DateTimeOffset.UtcNow.AddHours(8)});
 }
}
