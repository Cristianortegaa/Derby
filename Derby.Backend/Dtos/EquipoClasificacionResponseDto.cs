namespace Derby.Backend.Dtos;

public class EquipoClasificacionResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int PartidosJugados { get; set; }
    public int Ganancias { get; set; }
    public int Empates { get; set; }
    public int Derrotas { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }
    public int Puntos { get; set; }
}

