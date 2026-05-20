namespace Derby.Backend.Models;

public class Liga
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CompeticionId { get; set; }
    
    public string Grupo { get; set; } = "Único";
    public int Jornadas { get; set; }
    public int JornadaActual { get; set; }
    public string Estado { get; set; } = "Activo";
}