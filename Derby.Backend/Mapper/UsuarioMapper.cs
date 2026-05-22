using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class UsuarioMapper
{
    public static UsuarioResponseDto ToDto(this Usuario usuario, string token)
    {
        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Token = token,
            ArbitroId = usuario.ArbitroId
        };
    }
}
