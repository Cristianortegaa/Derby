namespace Derby.Backend.Dtos;

public class JugadorResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Dorsal { get; set; }
    public int EquipoId { get; set; }
}