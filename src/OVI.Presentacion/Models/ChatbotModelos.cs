namespace OVI.Presentacion.Models;

public class ContextoVocacionalDto
{
    public string PerfilDominante { get; set; } = string.Empty;
    public List<RecomendacionContextoDto> Recomendaciones { get; set; } = new();
    public DateTime FechaGeneracion { get; set; }
}

public class RecomendacionContextoDto
{
    public int Posicion { get; set; }
    public string NombreCarrera { get; set; } = string.Empty;
    public double PorcentajeAfinidad { get; set; }
}

public record TurnoConversacion(string Rol, string Contenido);

public static class SessionKeys
{
    public static string ConversacionIA(int idUsuario, int idResultado) =>
        $"OVI_ConversacionIA_{idUsuario}_{idResultado}";
}
