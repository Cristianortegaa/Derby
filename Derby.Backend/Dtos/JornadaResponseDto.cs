namespace Derby.Backend.Dtos;

public class JornadaResponseDto
{
    public int Numero { get; set; }
    public List<PartidoResponseDto> Partidos { get; set; } = new();
}

