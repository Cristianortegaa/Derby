namespace Derby.Backend.Models;

public class Partido
{
    public int Id { get; set; }
    public DateTime? FechaHora { get; set; }

    public int Jornada { get; set; }

    public int LigaId { get; set; }
    public Liga? Liga { get; set; }

    public int EquipoLocalId { get; set; }
    public Equipo? EquipoLocal { get; set; }

    public int EquipoVisitanteId { get; set; }
    public Equipo? EquipoVisitante { get; set; }

    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }

    public string Estado { get; set; } = "Pendiente";

    public int? ArbitroId { get; set; }
    public Arbitro? Arbitro { get; set; }
}