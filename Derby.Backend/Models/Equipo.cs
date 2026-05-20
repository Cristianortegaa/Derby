using System.ComponentModel.DataAnnotations.Schema;

namespace Derby.Backend.Models;

public class Equipo
{
    public int Id { get; set; }
    
    public string Nombre { get; set; } = string.Empty;
    
    public string EscudoUrl { get; set; } = string.Empty;
    
    public string Sede { get; set; } = string.Empty;
    
    [NotMapped]
    public string? LigaNombre { get; set; }

    public string Entrenador { get; set; } = string.Empty;

    public List<Jugador> Jugadores { get; set; } = new();
}