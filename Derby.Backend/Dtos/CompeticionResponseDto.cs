namespace Derby.Backend.Dtos;

public class CompeticionResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Temporada { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? TipoJuego { get; set; }
    public string? Grupo { get; set; }
}
