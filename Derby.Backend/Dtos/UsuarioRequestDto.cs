﻿﻿using System.Text.Json.Serialization;

namespace Derby.Backend.Dtos;

public class UsuarioRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("contrasena")]
    public string Contrasena { get; set; } = string.Empty;
    
    [JsonPropertyName("rol")]
    public string Rol { get; set; } = "Aficionado";
}

public class UsuarioResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class RegistroRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("contrasena")]
    public string Contrasena { get; set; } = string.Empty;
    
    [JsonPropertyName("rol")]
    public string Rol { get; set; } = "Aficionado";
}

