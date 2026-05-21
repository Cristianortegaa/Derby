namespace Derby.Backend.Dtos;

public class EventoPartidoResponseDto
{
    public int Id { get; set; }
    public int Minuto { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public int JugadorId { get; set; }
    public string NombreJugador { get; set; } = string.Empty;
    public int PartidoId { get; set; }
}