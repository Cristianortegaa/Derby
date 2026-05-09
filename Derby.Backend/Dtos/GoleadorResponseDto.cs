namespace Derby.Backend.Dtos;

public class GoleadorResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public int Goles { get; set; }
}

