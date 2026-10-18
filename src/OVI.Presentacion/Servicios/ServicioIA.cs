
using System.Net.Http.Json;
using System.Text.Json;

namespace OVI.Presentacion.Servicios;

public class ServicioIA(
    HttpClient http,
    IConfiguration config,
    ILogger<ServicioIA> log)
{
    // Configuración de Microsoft Foundry
    


    public bool Configurada =>
        !string.IsNullOrWhiteSpace(config["IA:ApiKey"]) &&
        !string.IsNullOrWhiteSpace(config["IA:Endpoint"]) &&
        !string.IsNullOrWhiteSpace(config["IA:Model"]);

    public async Task<string?> PreguntarAsync(
        string mensaje,
        string? contexto,
        CancellationToken ct)
    {
        if (!Configurada)
        {
            log.LogWarning("La API de Microsoft Foundry no está configurada.");
            return null;
        }

        var endpoint = config["IA:Endpoint"]!;
        var apiKey = config["IA:ApiKey"]!;
        var modelo = config["IA:Model"]!;

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps)
        {
            log.LogWarning("El endpoint de Microsoft Foundry no es válido.");
            return null;
        }

        using var req = new HttpRequestMessage(HttpMethod.Post, uri);

        // Autenticación mediante API Key de Azure
        req.Headers.Add("api-key", apiKey);

        req.Content = JsonContent.Create(new
        {
            model = modelo,
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content =
                        "Eres el asistente virtual de Orientación Vocacional OVI, " +
                        "una aplicación educativa para estudiantes de El Salvador. " +
                        "Ayuda a los estudiantes a descubrir carreras universitarias " +
                        "según sus intereses, habilidades y resultados del test RIASEC. " +
                        "Responde en español de forma clara, amable y profesional. " +
                        "Explica las carreras, sus áreas de estudio y posibles " +
                        "oportunidades laborales. " +
                        "No garantices admisión universitaria ni empleo. " +
                        "No realices diagnósticos psicológicos. " +
                        "No solicites información sensible. " +
                        "Si no tienes información suficiente, acláralo. " +
                        "Contexto del estudiante: " +
                        (contexto ?? "No disponible.")
                },
                new
                {
                    role = "user",
                    content = mensaje
                }
            },
            temperature = 0.4,
            max_tokens = 450
        });

        try
        {
            using var resp = await http.SendAsync(req, ct);

            if (!resp.IsSuccessStatusCode)
            {
                var detalle = await resp.Content.ReadAsStringAsync(ct);

                log.LogWarning(
                    "Microsoft Foundry devolvió HTTP {Estado}. Detalle: {Detalle}",
                    (int)resp.StatusCode,
                    detalle.Length > 500 ? detalle[..500] : detalle);

                return null;
            }

            using var json = await JsonDocument.ParseAsync(
                await resp.Content.ReadAsStreamAsync(ct),
                cancellationToken: ct);

            return json.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            TaskCanceledException or
            JsonException or
            KeyNotFoundException or
            InvalidOperationException)
        {
            log.LogWarning(
                ex,
                "Error al consultar Microsoft Foundry.");

            return null;
        }
    }
}
