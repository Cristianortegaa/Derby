namespace Derby.Backend.Dtos;

public class LigaResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CompeticionId { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public int Jornadas { get; set; }
    public int JornadaActual { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int TotalPartidos { get; set; }
    public int PartidosFinalizados { get; set; }
}
