namespace Derby.Backend.Dtos;

public class ArbitroRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroColegiado { get; set; } = string.Empty;
}

public class ArbitroResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroColegiado { get; set; } = string.Empty;
}

