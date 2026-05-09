using Derby.Backend.Dtos;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class CompeticionService : ICompeticionService
{
    private readonly ICompeticionRepository _competicionRepository;
    private readonly IPartidoRepository _partidoRepository;

    public CompeticionService(ICompeticionRepository competicionRepository, IPartidoRepository partidoRepository)
    {
        _competicionRepository = competicionRepository;
        _partidoRepository = partidoRepository;
    }

    public async Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int competicionId)
    {
        var partidos = await _partidoRepository.ObtenerPorCompeticionAsync(competicionId);
        
        var jornadas = partidos
            .GroupBy(p => p.Jornada)
            .OrderBy(g => g.Key)
            .Select(g => new JornadaResponseDto
            {
                Numero = g.Key,
                Partidos = g.Select(p => new PartidoResponseDto
                {
                    Id = p.Id,
                    Fecha = p.Fecha,
                    GolesLocal = p.GolesLocal ?? 0,
                    GolesVisitantes = p.GolesVisitantes ?? 0,
                    Estado = DeterminarEstado(p),
                    EquipoLocal = new EquipoResponseDto
                    {
                        Id = p.EquipoLocal?.Id ?? 0,
                        Nombre = p.EquipoLocal?.Nombre ?? "Desconocido",
                        EscudoUrl = p.EquipoLocal?.EscudoUrl ?? "",
                        Sede = p.EquipoLocal?.Sede ?? "",
                        Division = p.EquipoLocal?.Division ?? ""
                    },
                    EquipoVisitante = new EquipoResponseDto
                    {
                        Id = p.EquipoVisitante?.Id ?? 0,
                        Nombre = p.EquipoVisitante?.Nombre ?? "Desconocido",
                        EscudoUrl = p.EquipoVisitante?.EscudoUrl ?? "",
                        Sede = p.EquipoVisitante?.Sede ?? "",
                        Division = p.EquipoVisitante?.Division ?? ""
                    }
                }).ToList()
            }).ToList();

        return jornadas;
    }

    public async Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int competicionId)
    {
        var partidos = await _partidoRepository.ObtenerResultadosAsync(competicionId);

        var resultados = partidos
            .Select(p => new ResultadoPartidoResponseDto
            {
                Id = p.Id,
                EquipoLocal = p.EquipoLocal?.Nombre ?? "Desconocido",
                EquipoVisitante = p.EquipoVisitante?.Nombre ?? "Desconocido",
                GolesLocal = p.GolesLocal ?? 0,
                GolesVisitante = p.GolesVisitantes ?? 0,
                Fecha = p.Fecha
            }).ToList();

        return resultados;
    }

    public async Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int competicionId)
    {
        var partidos = await _partidoRepository.ObtenerResultadosAsync(competicionId);

        var clasificacion = new Dictionary<int, EquipoClasificacionResponseDto>();

        foreach (var partido in partidos)
        {
            var equipoLocal = partido.EquipoLocal;
            var equipoVisitante = partido.EquipoVisitante;

            if (equipoLocal != null)
            {
                if (!clasificacion.ContainsKey(equipoLocal.Id))
                {
                    clasificacion[equipoLocal.Id] = new EquipoClasificacionResponseDto
                    {
                        Id = equipoLocal.Id,
                        Nombre = equipoLocal.Nombre,
                        PartidosJugados = 0,
                        Ganancias = 0,
                        Empates = 0,
                        Derrotas = 0,
                        GolesAFavor = 0,
                        GolesEnContra = 0,
                        Puntos = 0
                    };
                }

                var stats = clasificacion[equipoLocal.Id];
                stats.PartidosJugados++;
                stats.GolesAFavor += partido.GolesLocal ?? 0;
                stats.GolesEnContra += partido.GolesVisitantes ?? 0;

                if (partido.GolesLocal > partido.GolesVisitantes)
                {
                    stats.Ganancias++;
                    stats.Puntos += 3;
                }
                else if (partido.GolesLocal == partido.GolesVisitantes)
                {
                    stats.Empates++;
                    stats.Puntos += 1;
                }
                else
                {
                    stats.Derrotas++;
                }
            }

            if (equipoVisitante != null)
            {
                if (!clasificacion.ContainsKey(equipoVisitante.Id))
                {
                    clasificacion[equipoVisitante.Id] = new EquipoClasificacionResponseDto
                    {
                        Id = equipoVisitante.Id,
                        Nombre = equipoVisitante.Nombre,
                        PartidosJugados = 0,
                        Ganancias = 0,
                        Empates = 0,
                        Derrotas = 0,
                        GolesAFavor = 0,
                        GolesEnContra = 0,
                        Puntos = 0
                    };
                }

                var stats = clasificacion[equipoVisitante.Id];
                stats.PartidosJugados++;
                stats.GolesAFavor += partido.GolesVisitantes ?? 0;
                stats.GolesEnContra += partido.GolesLocal ?? 0;

                if (partido.GolesVisitantes > partido.GolesLocal)
                {
                    stats.Ganancias++;
                    stats.Puntos += 3;
                }
                else if (partido.GolesVisitantes == partido.GolesLocal)
                {
                    stats.Empates++;
                    stats.Puntos += 1;
                }
                else
                {
                    stats.Derrotas++;
                }
            }
        }

        return clasificacion.Values
            .OrderByDescending(e => e.Puntos)
            .ThenByDescending(e => e.GolesAFavor - e.GolesEnContra)
            .ThenByDescending(e => e.GolesAFavor)
            .ToList();
    }

    public async Task<List<GoleadorResponseDto>> ObtenerGoleadoresAsync(int competicionId)
    {
        // Placeholder - necesitaría una tabla de goles por jugador
        return new List<GoleadorResponseDto>();
    }

    public async Task<List<JornadaResponseDto>> BuscarCompeticionesAsync(string? temporada, string? tipoJuego, string? competicion, string? grupo)
    {
        var competiciones = await _competicionRepository.FiltrarAsync(temporada, tipoJuego, competicion, grupo);
        
        var jornadas = new List<JornadaResponseDto>();
        foreach (var comp in competiciones)
        {
            var jornadasComp = await ObtenerJornadasAsync(comp.Id);
            jornadas.AddRange(jornadasComp);
        }

        return jornadas;
    }

    private string DeterminarEstado(Models.Partido partido)
    {
        if (partido.Finalizado == true)
            return "jugado";
        
        if (DateTime.Now > partido.Fecha)
            return "en-juego";
        
        return "pendiente";
    }
}

