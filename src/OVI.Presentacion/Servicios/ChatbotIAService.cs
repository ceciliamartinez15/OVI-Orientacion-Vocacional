using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OVI.Presentacion.Models;

namespace OVI.Presentacion.Servicios;

public interface IChatbotIAService
{
    Task<string> ObtenerRespuestaAsync(
        ContextoVocacionalDto? contexto,
        string mensajeUsuario,
        List<TurnoConversacion>? historial,
        CancellationToken cancellationToken = default);
}

public class ChatbotIAService : IChatbotIAService
{
    private const int MaxTurnosHistorial = 6;
    private const int TimeoutSegundos = 18;
    private const int MaxTokensRespuesta = 400;
    private const double Temperatura = 0.5;

    private const string MensajeErrorGenerico =
        "Tuve un problema para conectarme con el asistente, intenta de nuevo en unos momentos.";
    private const string MensajeTimeout =
        "El asistente tardó demasiado en responder. Intenta de nuevo con un mensaje más corto.";
    private const string MensajeSinResultado =
        "Todavía no tengo tus resultados del test vocacional. Complétalo primero para poder " +
        "orientarte sobre tu perfil y tus carreras recomendadas.";
    private const string MensajeNoConfigurado =
        "El chat con inteligencia artificial todavía no está configurado. Mientras tanto, " +
        "puedes consultar las preguntas frecuentes.";

    private readonly HttpClient _httpClient;
    private readonly ChatbotIAOptions _opciones;
    private readonly ILogger<ChatbotIAService> _logger;

    public ChatbotIAService(HttpClient httpClient, IOptions<ChatbotIAOptions> opciones, ILogger<ChatbotIAService> logger)
    {
        _httpClient = httpClient;
        _opciones = opciones.Value;
        _logger = logger;
    }

    private bool Configurado =>
        !string.IsNullOrWhiteSpace(_opciones.ApiKey) &&
        !string.IsNullOrWhiteSpace(_opciones.Modelo) &&
        Uri.TryCreate(_opciones.Endpoint, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps;

    public async Task<string> ObtenerRespuestaAsync(
        ContextoVocacionalDto? contexto,
        string mensajeUsuario,
        List<TurnoConversacion>? historial,
        CancellationToken cancellationToken = default)
    {
        if (contexto == null || string.IsNullOrWhiteSpace(contexto.PerfilDominante))
        {
            return MensajeSinResultado;
        }

        if (string.IsNullOrWhiteSpace(mensajeUsuario))
        {
            return MensajeErrorGenerico;
        }

        if (!Configurado)
        {
            _logger.LogWarning("El chatbot IA no está configurado (sección ChatbotIA: Endpoint HTTPS, Modelo y ApiKey).");
            return MensajeNoConfigurado;
        }

        var systemPrompt = ConstruirSystemPrompt(contexto);
        var mensajes = ConstruirMensajes(systemPrompt, historial, mensajeUsuario);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSegundos));

        try
        {
            var payload = new
            {
                model = _opciones.Modelo,
                messages = mensajes,
                temperature = Temperatura,
                max_tokens = MaxTokensRespuesta
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _opciones.Endpoint)
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opciones.ApiKey);

            using var response = await _httpClient.SendAsync(request, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var detalle = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogError(
                    "ChatbotIA respondió {StatusCode}: {Detalle}",
                    (int)response.StatusCode,
                    detalle.Length > 500 ? detalle[..500] : detalle);
                return MensajeErrorGenerico;
            }

            var cuerpoRespuesta = await response.Content
                .ReadFromJsonAsync<OpenAiChatResponse>(cancellationToken: cts.Token);

            var textoRespuesta = cuerpoRespuesta?.Choices?.FirstOrDefault()?.Message?.Content;

            return string.IsNullOrWhiteSpace(textoRespuesta)
                ? MensajeErrorGenerico
                : textoRespuesta.Trim();
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Timeout al llamar al chatbot IA (>{Timeout}s).", TimeoutSegundos);
            return MensajeTimeout;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al llamar al chatbot IA.");
            return MensajeErrorGenerico;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al parsear la respuesta del chatbot IA.");
            return MensajeErrorGenerico;
        }
    }

    private static string ConstruirSystemPrompt(ContextoVocacionalDto contexto)
    {
        var listaCarreras = string.Join(
            "\n",
            contexto.Recomendaciones
                .OrderBy(r => r.Posicion)
                .Select(r => $"{r.Posicion}. {r.NombreCarrera} ({r.PorcentajeAfinidad}% de afinidad)"));

        return
$@"Eres Saviox, el asesor vocacional de VocalO para estudiantes de El Salvador.
Responde siempre en español, de forma clara, breve (máximo 4-5 líneas) y motivadora.

Estos son los resultados YA CALCULADOS de este estudiante en el test RIASEC. Son datos
reales que no debes modificar, inventar de nuevo ni contradecir:

Perfil dominante: {contexto.PerfilDominante}
Carreras recomendadas (en orden de afinidad):
{listaCarreras}

Reglas estrictas:
- No inventes un perfil dominante distinto ni carreras que no estén en la lista de arriba.
- Solo puedes hablar de orientación vocacional, las carreras recomendadas de arriba, el
  proceso de elegir carrera y temas relacionados con estudios superiores en El Salvador.
- Si te preguntan algo fuera de ese tema, declina amablemente y redirige la conversación
  hacia su orientación vocacional.
- No garantices admisión universitaria ni empleo.
- No realices diagnósticos psicológicos y no solicites información sensible.";
    }

    private static List<object> ConstruirMensajes(
        string systemPrompt, List<TurnoConversacion>? historial, string mensajeUsuario)
    {
        var mensajes = new List<object>
        {
            new { role = "system", content = systemPrompt }
        };

        if (historial != null && historial.Count > 0)
        {
            foreach (var turno in historial.TakeLast(MaxTurnosHistorial))
            {
                mensajes.Add(new { role = turno.Rol, content = turno.Contenido });
            }
        }

        mensajes.Add(new { role = "user", content = mensajeUsuario });

        return mensajes;
    }

    private class OpenAiChatResponse
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private class OpenAiChoice
    {
        public OpenAiMessage? Message { get; set; }
    }

    private class OpenAiMessage
    {
        public string? Content { get; set; }
    }
}
