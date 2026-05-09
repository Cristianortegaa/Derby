namespace Derby.Backend.Models;

public class EventoPartido
{
    public int Id { get; set; }
    public int Minuto { get; set; }
    public TipoEvento TipoEvento { get; set; }
    
    public int JugadorId { get; set; }
    public Jugador? Jugador { get; set; }
    
    public int PartidoId { get; set; }
    public Partido? Partido { get; set; }
}

public enum TipoEvento
{
    Gol,
    TarjetaAmarilla,
    TarjetaRoja
}