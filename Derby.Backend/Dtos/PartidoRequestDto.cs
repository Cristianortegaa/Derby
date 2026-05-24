namespace Derby.Backend.Dtos;

public class PartidoRequestDto
{
    public int Jornada { get; set; }
    public int LigaId { get; set; }
    public int EquipoLocalId { get; set; }
    public int EquipoVisitanteId { get; set; }
    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime? FechaHora { get; set; }
    public int? ArbitroId { get; set; }
}
