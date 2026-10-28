using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Presentacion.Models;
using OVI.Presentacion.Servicios;

namespace OVI.Presentacion.Controllers;

[Authorize]
public class ChatbotController(IChatbotFAQService faqService, IChatbotIAService iaService, OVIDbContext db) : Controller
{
    private const int MaxLongitudMensaje = 500;
    private const int MaxTurnosGuardados = 6;
    private const string MensajeInvalido = "Escribe una pregunta de máximo 500 caracteres.";

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Faq() => View();

    // Alimenta la burbuja flotante de Saviox (presente en todas las pantallas, también sin sesión).
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> FaqOpciones(CancellationToken cancellationToken)
        => Json(await faqService.ObtenerOpcionesAsync(cancellationToken));

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PreguntarFaq(string mensaje, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mensaje) || mensaje.Length > MaxLongitudMensaje)
        {
            return Json(new { respuesta = MensajeInvalido });
        }

        var respuesta = await faqService.ObtenerRespuestaAsync(mensaje, cancellationToken);
        return Json(new { respuesta });
    }

    [HttpGet]
    public async Task<IActionResult> Contextual(CancellationToken cancellationToken)
    {
        var (contexto, _) = await ObtenerContextoAsync(cancellationToken);

        ViewBag.HayResultado = contexto != null;
        ViewBag.PerfilDominante = contexto?.PerfilDominante;

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PreguntarIA(string mensaje, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mensaje) || mensaje.Length > MaxLongitudMensaje)
        {
            return Json(new { respuesta = MensajeInvalido });
        }

        var (contexto, idResultado) = await ObtenerContextoAsync(cancellationToken);
        var claveHistorial = SessionKeys.ConversacionIA(UsuarioActual.Id(User), idResultado);
        var historial = ObtenerHistorialDeSesion(claveHistorial);

        var respuesta = await iaService.ObtenerRespuestaAsync(contexto, mensaje, historial, cancellationToken);

        if (contexto != null)
        {
            historial.Add(new TurnoConversacion("user", mensaje));
            historial.Add(new TurnoConversacion("assistant", respuesta));
            GuardarHistorialEnSesion(claveHistorial, historial);
        }

        return Json(new { respuesta });
    }

    private async Task<(ContextoVocacionalDto? Contexto, int IdResultado)> ObtenerContextoAsync(CancellationToken cancellationToken)
    {
        var idUsuario = UsuarioActual.Id(User);

        var resultado = await db.Resultados
            .AsNoTracking()
            .Include(x => x.Recomendaciones).ThenInclude(x => x.Carrera)
            .Where(x => x.Evaluacion!.IdUsuario == idUsuario)
            .OrderByDescending(x => x.FechaResultado)
            .FirstOrDefaultAsync(cancellationToken);

        if (resultado == null)
        {
            return (null, 0);
        }

        var contexto = new ContextoVocacionalDto
        {
            PerfilDominante = resultado.PerfilDominante,
            FechaGeneracion = resultado.FechaResultado,
            Recomendaciones = resultado.Recomendaciones
                .OrderBy(r => r.Posicion)
                .Select(r => new RecomendacionContextoDto
                {
                    Posicion = r.Posicion,
                    NombreCarrera = r.Carrera?.Nombre ?? string.Empty,
                    PorcentajeAfinidad = (double)r.NivelAfinidad
                })
                .ToList()
        };

        return (contexto, resultado.IdResultado);
    }

    private List<TurnoConversacion> ObtenerHistorialDeSesion(string clave)
    {
        var json = HttpContext.Session.GetString(clave);
        return string.IsNullOrEmpty(json)
            ? new List<TurnoConversacion>()
            : JsonSerializer.Deserialize<List<TurnoConversacion>>(json) ?? new List<TurnoConversacion>();
    }

    private void GuardarHistorialEnSesion(string clave, List<TurnoConversacion> historial)
    {
        var turnosRecientes = historial.TakeLast(MaxTurnosGuardados).ToList();
        HttpContext.Session.SetString(clave, JsonSerializer.Serialize(turnosRecientes));
    }
}
