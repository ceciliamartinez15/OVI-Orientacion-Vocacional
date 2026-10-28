using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;

namespace OVI.Presentacion.Servicios;

public interface IChatbotFAQService
{
    Task<string> ObtenerRespuestaAsync(string mensajeUsuario, CancellationToken cancellationToken = default);

    /// <summary>Preguntas frecuentes listas para mostrarse como opciones (pregunta + respuesta).</summary>
    Task<IReadOnlyList<FaqOpcion>> ObtenerOpcionesAsync(CancellationToken cancellationToken = default);
}

public sealed record FaqOpcion(string Pregunta, string Respuesta);

public class ChatbotFAQService : IChatbotFAQService
{
    private const string RespuestaPorDefecto =
        "No encontré una respuesta exacta a tu pregunta. Intenta reformularla con otras " +
        "palabras, o usa el chat con inteligencia artificial para una respuesta más detallada.";

    private static readonly HashSet<string> Stopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        "de","la","el","que","y","en","un","una","es","por","para","con","mi","tu","su","sus",
        "los","las","se","del","al","lo","como","mas","pero","le","ya","o","este","esta",
        "entre","cuando","muy","sin","sobre","tambien","me","hasta","donde","quien","desde",
        "todo","nos","durante","todos","uno","les","ni","contra","otros","ese","eso","ante",
        "ellos","esto","antes","algunos","unos","yo","otra","otras","tanto","esa","estos",
        "mucho","quienes","nada","muchos","cual","poco","ella","estar","estas","algunas",
        "algo","nosotros","tengo","tiene","hola","quiero","saber","porque"
    };

    // Etiqueta: texto legible para mostrar como opción (Pregunta es una cadena de palabras clave para el buscador).
    private sealed record FaqItem(string Pregunta, string Respuesta, string? Etiqueta = null);

    private static readonly List<FaqItem> FaqsBase = new()
    {
        new("Cuanto dura el test vocacional cuanto tiempo toma",
            "El test de VocalO consiste en afirmaciones que calificas del 1 al 5, de «Nada» a «Muchísimo», y normalmente toma pocos minutos completarlo.", "¿Cuánto dura el test?"),
        new("Puedo repetir el test hacerlo de nuevo otra vez",
            "Sí, puedes repetir el test las veces que quieras. Cada intento queda guardado en tu historial y genera nuevas recomendaciones según tus respuestas.", "¿Puedo repetir el test?"),
        new("Que significa mi perfil dominante RIASEC letras",
            "Tu perfil dominante son las áreas RIASEC (Realista, Investigador, Artístico, Social, Emprendedor, Convencional) donde obtuviste más puntaje. Reflejan tus intereses y tu forma preferida de trabajar.", "¿Qué significa mi perfil dominante?"),
        new("Como se calcula el porcentaje afinidad de una carrera",
            "Sumamos tus puntajes en las áreas RIASEC asociadas a cada carrera y el resultado se expresa como porcentaje de afinidad.", "¿Cómo se calcula la afinidad de una carrera?"),
        new("Cuantas carreras recomienda muestra VocalO en total",
            "VocalO te recomienda las 3 carreras con mayor afinidad según tus respuestas, entre carreras universitarias y técnicas.", "¿Cuántas carreras me recomiendan?"),
        new("Es gratis usar VocalO tiene algun costo",
            "Sí, usar la plataforma VocalO y realizar el test vocacional es completamente gratuito.", "¿Usar VocalO tiene algún costo?"),
        new("Puedo hacer preguntas sobre mis resultados carreras",
            "Sí, una vez tengas tus resultados puedes usar el chat con inteligencia artificial (opción Asistente) para preguntar sobre tu perfil, tus carreras recomendadas y su campo laboral.", "¿Puedo hacer preguntas sobre mis resultados?"),
        new("Necesito crear una cuenta registrarme para hacer el test",
            "Sí, necesitas crear una cuenta gratuita para realizar el test. Así tus resultados quedan guardados en tu historial y solo tú puedes verlos.", "¿Necesito una cuenta para hacer el test?")
    };

    private readonly OVIDbContext _db;

    public ChatbotFAQService(OVIDbContext db)
    {
        _db = db;
    }

    public async Task<string> ObtenerRespuestaAsync(string mensajeUsuario, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mensajeUsuario))
        {
            return RespuestaPorDefecto;
        }

        var palabrasClaveUsuario = ExtraerPalabrasClave(mensajeUsuario);
        if (palabrasClaveUsuario.Count == 0)
        {
            return RespuestaPorDefecto;
        }

        var faqsBd = await _db.FAQChatbot
            .AsNoTracking()
            .Where(f => f.Estado)
            .ToListAsync(cancellationToken);

        var candidatas = faqsBd
            .Select(f => new FaqItem(
                f.Pregunta + " " + (f.PalabrasClave ?? string.Empty).Replace(',', ' '),
                f.Respuesta))
            .Concat(FaqsBase);

        FaqItem? mejorCoincidencia = null;
        int mejorPuntaje = 0;

        foreach (var faq in candidatas)
        {
            var palabrasClavePregunta = ExtraerPalabrasClave(faq.Pregunta);
            int coincidencias = palabrasClaveUsuario.Intersect(palabrasClavePregunta).Count();

            if (coincidencias > mejorPuntaje)
            {
                mejorPuntaje = coincidencias;
                mejorCoincidencia = faq;
            }
        }

        return mejorPuntaje > 0 && mejorCoincidencia != null
            ? mejorCoincidencia.Respuesta
            : RespuestaPorDefecto;
    }

    public async Task<IReadOnlyList<FaqOpcion>> ObtenerOpcionesAsync(CancellationToken cancellationToken = default)
    {
        var faqsBd = await _db.FAQChatbot
            .AsNoTracking()
            .Where(f => f.Estado)
            .OrderBy(f => f.IdFAQ)
            .ToListAsync(cancellationToken);

        var desdeBd = faqsBd.Select(f => new FaqOpcion(f.Pregunta, f.Respuesta));
        var desdeBase = FaqsBase.Select(f => new FaqOpcion(f.Etiqueta ?? f.Pregunta, f.Respuesta));

        return desdeBd
            .Concat(desdeBase)
            .DistinctBy(o => Normalizar(o.Pregunta))
            .ToList();
    }

    private static List<string> ExtraerPalabrasClave(string texto)
    {
        var normalizado = Normalizar(texto);

        return normalizado
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length > 2 && !Stopwords.Contains(p))
            .Distinct()
            .ToList();
    }

    private static string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        var formaDescompuesta = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var caracter in formaDescompuesta)
        {
            var categoriaUnicode = CharUnicodeInfo.GetUnicodeCategory(caracter);

            if (categoriaUnicode == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(caracter) || char.IsWhiteSpace(caracter))
            {
                builder.Append(caracter);
            }
            else
            {
                builder.Append(' ');
            }
        }

        var textoLimpio = builder.ToString().Normalize(NormalizationForm.FormC);
        return Regex.Replace(textoLimpio, @"\s+", " ").Trim();
    }
}
