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
            LigaId = partido.LigaId,
            LigaNombre = partido.Liga?.Nombre,
            Jornada = partido.Jornada,
            FechaHora = partido.FechaHora,
            GolesLocal = partido.GolesLocal,
            GolesVisitante = partido.GolesVisitante,
            Estado = partido.Estado,
            ArbitroId = partido.ArbitroId,
            EquipoLocalId = partido.EquipoLocalId,
            EquipoVisitanteId = partido.EquipoVisitanteId,
            EquipoLocal = partido.EquipoLocal?.ToDto(),
            EquipoVisitante = partido.EquipoVisitante?.ToDto(),
            ArbitroNombre = partido.Arbitro?.Nombre,
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
            Fecha = partido.FechaHora ?? DateTime.MinValue,
            EscudoLocalUrl = partido.EquipoLocal?.EscudoUrl ?? "",
            EscudoVisitanteUrl = partido.EquipoVisitante?.EscudoUrl ?? ""
        };
    }
}
