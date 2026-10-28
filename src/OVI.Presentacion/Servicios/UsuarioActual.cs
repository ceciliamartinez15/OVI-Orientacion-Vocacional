using System.Security.Claims;
namespace OVI.Presentacion.Servicios;
public static class UsuarioActual
{
 public static int Id(ClaimsPrincipal user) => int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:0;
}
