namespace Derby.Backend.Dtos;

public class ResultadoPartidoResponseDto
{
    public int Id { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
    public int GolesLocal { get; set; }
    public int GolesVisitante { get; set; }
    public DateTime Fecha { get; set; }
}
