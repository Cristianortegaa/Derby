namespace Derby.Backend.Dtos;

public class EquipoResponseDto
{
    public int Id { get; set; } 
    
    public string Nombre { get; set; } = string.Empty;
    
    public string EscudoUrl { get; set; } = string.Empty;
    
    public string Sede { get; set; } = string.Empty;
    
    public string Division { get; set; } = string.Empty;
}