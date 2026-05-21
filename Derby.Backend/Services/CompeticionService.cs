using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class CompeticionService : ICompeticionService
{
    private readonly ICompeticionRepository _competicionRepository;
    private readonly IPartidoRepository _partidoRepository;
    private readonly IEventoPartidoRepository _eventoRepository;

    public CompeticionService(ICompeticionRepository competicionRepository, IPartidoRepository partidoRepository, IEventoPartidoRepository eventoRepository)
    {
        _competicionRepository = competicionRepository;
        _partidoRepository = partidoRepository;
        _eventoRepository = eventoRepository;
    }

    public async Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int competicionId)
    {
        var partidos = await _partidoRepository.ObtenerPorCompeticionAsync(competicionId);
        return partidos
            .GroupBy(p => p.Jornada)
            .OrderBy(g => g.Key)
            .Select(g => new JornadaResponseDto
            {
                Numero = g.Key,
                Partidos = g.Select(p => p.ToDto()).ToList()
            }).ToList();
    }

    public async Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int competicionId)
    {
        var partidos = await _partidoRepository.ObtenerResultadosAsync(competicionId);
        return partidos.Select(p => p.ToResultadoDto()).ToList();
    }

    public async Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int competicionId)
    {
        var partidos = await _partidoRepository.ObtenerResultadosAsync(competicionId);
        return CalcularClasificacion(partidos);
    }

    public async Task<List<GoleadorResponseDto>> ObtenerGoleadoresAsync(int competicionId)
    {
        var goles = await _eventoRepository.ObtenerGolesPorCompeticionAsync(competicionId);

        return goles
            .GroupBy(e => e.JugadorId)
            .Select(g => new GoleadorResponseDto
            {
                Id = g.Key,
                Nombre = g.First().Jugador?.Nombre ?? "Desconocido",
                Equipo = g.First().Jugador?.Equipo?.Nombre ?? "Desconocido",
                Goles = g.Count()
            })
            .OrderByDescending(g => g.Goles)
            .ToList();
    }

    public async Task<List<Competicion>> BuscarCompeticionesAsync(string? temporada, string? tipoJuego, string? competicion, string? grupo)
    {
        return await _competicionRepository.FiltrarAsync(temporada, tipoJuego, competicion, grupo);
    }

    private static List<EquipoClasificacionResponseDto> CalcularClasificacion(List<Partido> partidos)
    {
        var clasificacion = new Dictionary<int, EquipoClasificacionResponseDto>();

        foreach (var partido in partidos)
        {
            var local = partido.EquipoLocal;
            var visitante = partido.EquipoVisitante;

            if (local != null)
            {
                if (!clasificacion.ContainsKey(local.Id))
                    clasificacion[local.Id] = new EquipoClasificacionResponseDto { Id = local.Id, Nombre = local.Nombre };

                var s = clasificacion[local.Id];
                s.PartidosJugados++;
                s.GolesAFavor += partido.GolesLocal ?? 0;
                s.GolesEnContra += partido.GolesVisitante ?? 0;
                if (partido.GolesLocal > partido.GolesVisitante) { s.Ganancias++; s.Puntos += 3; }
                else if (partido.GolesLocal == partido.GolesVisitante) { s.Empates++; s.Puntos += 1; }
                else s.Derrotas++;
            }

            if (visitante != null)
            {
                if (!clasificacion.ContainsKey(visitante.Id))
                    clasificacion[visitante.Id] = new EquipoClasificacionResponseDto { Id = visitante.Id, Nombre = visitante.Nombre };

                var s = clasificacion[visitante.Id];
                s.PartidosJugados++;
                s.GolesAFavor += partido.GolesVisitante ?? 0;
                s.GolesEnContra += partido.GolesLocal ?? 0;
                if (partido.GolesVisitante > partido.GolesLocal) { s.Ganancias++; s.Puntos += 3; }
                else if (partido.GolesVisitante == partido.GolesLocal) { s.Empates++; s.Puntos += 1; }
                else s.Derrotas++;
            }
        }

        return clasificacion.Values
            .OrderByDescending(e => e.Puntos)
            .ThenByDescending(e => e.GolesAFavor - e.GolesEnContra)
            .ThenByDescending(e => e.GolesAFavor)
            .ToList();
    }
}
