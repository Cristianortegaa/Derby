namespace Derby.Backend.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
    public Rol Rol { get; set; } 
    
    public int? ArbitroId { get; set; }
    public Arbitro? Arbitro { get; set; }
}

public enum Rol
{
    Administrador,
    Arbitro,
    Aficionado 
}