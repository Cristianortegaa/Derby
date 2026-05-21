namespace Derby.Backend.Dtos;

public class EventoPartidoRequestDto
{
    public int JugadorId { get; set; }
    public int Minuto { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
}