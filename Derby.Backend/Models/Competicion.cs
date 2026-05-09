﻿namespace Derby.Backend.Models;

public class Competicion
{
    public int Id { get; set; }
    
    public string Nombre { get; set; } = string.Empty; 
    
    public string Temporada { get; set; } = "2025/2026";
    
    public string? Descripcion { get; set; } // Descripción de la competición
    
    public string Estado { get; set; } = "Activo"; // Activo, Inactivo, Pausado, Finalizado
    
    public string? TipoJuego { get; set; } // futbol11, futbol7, futsal
    
    public string? Grupo { get; set; } // Grupo A, Grupo B, etc.
    
    public List<Partido> Partidos { get; set; } = new();
}