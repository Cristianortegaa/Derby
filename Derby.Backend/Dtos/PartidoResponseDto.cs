namespace Derby.Backend.Dtos;

public class PartidoResponseDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int? GolesLocal { get; set; }
    public int? GolesVisitantes { get; set; }
    public string Estado { get; set; } = string.Empty;
    public EquipoResponseDto? EquipoLocal { get; set; }
    public EquipoResponseDto? EquipoVisitante { get; set; }
}
