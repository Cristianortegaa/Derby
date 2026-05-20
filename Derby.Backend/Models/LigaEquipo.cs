namespace Derby.Backend.Models;

public class LigaEquipo
{
    public int Id { get; set; }
    public int LigaId { get; set; }
    public int EquipoId { get; set; }
    
    public Liga? Liga { get; set; }
    public Equipo? Equipo { get; set; }
}