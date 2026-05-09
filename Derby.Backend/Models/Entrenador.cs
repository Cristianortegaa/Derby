namespace Derby.Backend.Models;

public class Entrenador
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Licencia { get; set; } = string.Empty;
    
    public int EquipoId { get; set; }
    public Equipo? Equipo { get; set; }
    
}