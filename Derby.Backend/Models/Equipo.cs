namespace Derby.Backend.Models;

public class Equipo
{
    public int Id { get; set; }
    
    public string Nombre { get; set; } = string.Empty;
    
    public string EscudoUrl { get; set; } = string.Empty;
    
    public string Sede { get; set; } = string.Empty; 
    
    public string Division { get; set; } = string.Empty; 
    
    public List<Jugador> Jugadores { get; set; } = new();
    public List<Entrenador> Entrenadores { get; set; } = new();
}