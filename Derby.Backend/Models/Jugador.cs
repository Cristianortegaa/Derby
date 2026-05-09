namespace Derby.Backend.Models;

public class Jugador
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Dorsal { get; set; }
    
    public int EquipoId { get; set; }
    public Equipo? Equipo { get; set; }
}