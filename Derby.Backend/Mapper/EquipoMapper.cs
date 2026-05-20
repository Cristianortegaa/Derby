using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class EquipoMapper
{
    public static EquipoResponseDto ToDto(this Equipo equipo)
    {
        return new EquipoResponseDto
        {
            Id = equipo.Id,
            Nombre = equipo.Nombre,
            EscudoUrl = equipo.EscudoUrl,
            Sede = equipo.Sede,
            Entrenador = equipo.Entrenador,
            LigaNombre = equipo.LigaNombre
        };
    }

    public static Equipo ToEntity(this EquipoRequestDto dto)
    {
        return new Equipo
        {
            Nombre = dto.Nombre,
            EscudoUrl = dto.EscudoUrl,
            Sede = dto.Sede,
            Entrenador = dto.Entrenador
        };
    }
}