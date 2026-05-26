namespace Derby.Backend.Dtos;

public class PartidoResponseDto
{
    public int Id { get; set; }
    public int LigaId { get; set; }
    public string? LigaNombre { get; set; }
    public int Jornada { get; set; }
    public DateTime? FechaHora { get; set; }
    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int? ArbitroId { get; set; }
    public int EquipoLocalId { get; set; }
    public int EquipoVisitanteId { get; set; }
    public EquipoResponseDto? EquipoLocal { get; set; }
    public EquipoResponseDto? EquipoVisitante { get; set; }
    public string? ArbitroNombre { get; set; }
}
