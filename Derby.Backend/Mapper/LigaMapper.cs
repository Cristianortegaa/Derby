using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class LigaMapper
{
    public static LigaResponseDto ToDto(this Liga liga)
    {
        return new LigaResponseDto
        {
            Id = liga.Id,
            Nombre = liga.Nombre,
            CompeticionId = liga.CompeticionId,
            Grupo = liga.Grupo,
            Jornadas = liga.Jornadas,
            JornadaActual = liga.JornadaActual,
            Estado = liga.Estado,
            TotalPartidos = liga.Partidos.Count,
            PartidosFinalizados = liga.Partidos.Count(p => p.Estado == "Finalizado")
        };
    }
}
