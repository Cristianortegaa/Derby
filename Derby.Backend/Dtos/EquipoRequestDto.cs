using System.ComponentModel.DataAnnotations;

namespace Derby.Backend.Dtos;

public class EquipoRequestDto
{

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public string EscudoUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "La sede es obligatoria.")]
    public string Sede { get; set; } = string.Empty;

    public string Entrenador { get; set; } = string.Empty;

}