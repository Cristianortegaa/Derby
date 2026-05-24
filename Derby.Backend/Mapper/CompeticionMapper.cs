using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class CompeticionMapper
{
    public static CompeticionResponseDto ToDto(this Competicion competicion)
    {
        return new CompeticionResponseDto
        {
            Id = competicion.Id,
            Nombre = competicion.Nombre,
            Temporada = competicion.Temporada,
            Descripcion = competicion.Descripcion,
            Estado = competicion.Estado,
            TipoJuego = competicion.TipoJuego,
            Grupo = competicion.Grupo
        };
    }
}
