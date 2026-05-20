using System.ComponentModel.DataAnnotations;

namespace Derby.Backend.Dtos;

public class JugadorRequestDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
    
    [Range(1, 99)]
    public int Dorsal { get; set; }
}