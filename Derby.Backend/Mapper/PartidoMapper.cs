using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class PartidoMapper
{
    public static PartidoResponseDto ToDto(this Partido partido)
    {
        return new PartidoResponseDto
        {
            Id = partido.Id,
            Fecha = partido.FechaHora ?? DateTime.MinValue,
            GolesLocal = partido.GolesLocal,
            GolesVisitantes = partido.GolesVisitante,
            Estado = partido.Estado,
            EquipoLocal = partido.EquipoLocal?.ToDto(),
            EquipoVisitante = partido.EquipoVisitante?.ToDto()
        };
    }

    public static ResultadoPartidoResponseDto ToResultadoDto(this Partido partido)
    {
        return new ResultadoPartidoResponseDto
        {
            Id = partido.Id,
            EquipoLocal = partido.EquipoLocal?.Nombre ?? "Desconocido",
            EquipoVisitante = partido.EquipoVisitante?.Nombre ?? "Desconocido",
            GolesLocal = partido.GolesLocal ?? 0,
            GolesVisitante = partido.GolesVisitante ?? 0,
            Fecha = partido.FechaHora ?? DateTime.MinValue
        };
    }
}
