﻿namespace Derby.Backend.Models;

public class Competicion
{
    public int Id { get; set; }
    
    public string Nombre { get; set; } = string.Empty; 
    
    public string Temporada { get; set; } = "2025/2026";
    
    public string? Descripcion { get; set; } 
    
    public string Estado { get; set; } = "Activo"; 
    
    public string? TipoJuego { get; set; } 
    
    public string? Grupo { get; set; } 
    
}