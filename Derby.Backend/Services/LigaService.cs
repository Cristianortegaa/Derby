using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class LigaService : ILigaService
{
    private readonly ILigaRepository _ligaRepository;
    private readonly IPartidoRepository _partidoRepository;
    private readonly IEventoPartidoRepository _eventoRepository;

    public LigaService(ILigaRepository ligaRepository, IPartidoRepository partidoRepository,
        IEventoPartidoRepository eventoRepository)
    {
        _ligaRepository = ligaRepository;
        _partidoRepository = partidoRepository;
        _eventoRepository = eventoRepository;
    }

    public async Task<List<Equipo>> ObtenerEquiposAsync(int ligaId)
    {
        return await _ligaRepository.ObtenerEquiposAsync(ligaId);
    }

    public async Task<List<Equipo>> ObtenerEquiposSinLigaAsync()
    {
        return await _ligaRepository.ObtenerEquiposSinLigaAsync();
    }

    public async Task AgregarEquipoAsync(int ligaId, int equipoId)
    {
        if (await _ligaRepository.EquipoExisteAsync(ligaId, equipoId))
            throw new Exception("El equipo ya está en esta liga");

        var equiposLiga = await _ligaRepository.ObtenerEquiposAsync(ligaId);
        if (equiposLiga.Count >= 20)
            throw new Exception("La liga ya tiene el máximo de 20 equipos");

        var equiposSinLiga = await _ligaRepository.ObtenerEquiposSinLigaAsync();
        if (!equiposSinLiga.Any(e => e.Id == equipoId))
            throw new Exception("El equipo ya pertenece a otra liga");

        await _ligaRepository.AgregarEquipoAsync(ligaId, equipoId);
    }

    public async Task QuitarEquipoAsync(int ligaId, int equipoId)
    {
        await _ligaRepository.QuitarEquipoAsync(ligaId, equipoId);
    }

    public async Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int ligaId)
    {
        var partidos = await _partidoRepository.ObtenerPorLigaAsync(ligaId);
        return partidos
            .GroupBy(p => p.Jornada)
            .OrderBy(g => g.Key)
            .Select(g => new JornadaResponseDto
            {
                Numero = g.Key,
                Partidos = g.Select(p => p.ToDto()).ToList()
            }).ToList();
    }

    public async Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int ligaId)
    {
        var partidos = await _partidoRepository.ObtenerResultadosPorLigaAsync(ligaId);
        return partidos.Select(p => p.ToResultadoDto()).ToList();
    }

    public async Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int ligaId)
    {
        var partidos = await _partidoRepository.ObtenerResultadosPorLigaAsync(ligaId);
        return CalcularClasificacion(partidos);
    }

    public async Task<List<GoleadorResponseDto>> ObtenerGoleadoresAsync(int ligaId)
    {
        var goles = await _eventoRepository.ObtenerGolesPorLigaAsync(ligaId);

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

    public async Task<object> GenerarCalendarioAsync(int ligaId)
    {
        var liga = await _ligaRepository.ObtenerPorIdAsync(ligaId);
        if (liga == null)
            throw new Exception("Liga no encontrada");

        await _partidoRepository.EliminarPorLigaAsync(ligaId);

        var equipos = await _ligaRepository.ObtenerEquiposAsync(ligaId);
        if (equipos.Count < 2)
            throw new Exception("Se necesitan al menos 2 equipos para generar el calendario");

        var lista = equipos.ToList();
        if (lista.Count % 2 != 0)
            lista.Add(null!);

        int n = lista.Count;
        var partidos = new List<Partido>();

        var hoyLocal = DateTime.Today;
        int diasHastaSabado = ((int)DayOfWeek.Saturday - (int)hoyLocal.DayOfWeek + 7) % 7;
        if (diasHastaSabado == 0) diasHastaSabado = 7;
        var fechaBaseLocal = hoyLocal.AddDays(diasHastaSabado); 

        for (int ronda = 0; ronda < n - 1; ronda++)
        {
            int jornadaIda = ronda + 1;
            int jornadaVuelta = ronda + 1 + (n - 1);

            for (int i = 0; i < n / 2; i++)
            {
                var local = lista[i];
                var visitante = lista[n - 1 - i];

                var fechaJornadaIdaLocal = fechaBaseLocal.AddDays(7 * (jornadaIda - 1));
                var fechaJornadaVueltaLocal = fechaBaseLocal.AddDays(7 * (jornadaVuelta - 1));

                int partidosPorDia = 5; // 14, 16, 18, 20, 22
                int indice = i;

                bool esDomingo = indice >= partidosPorDia;
                int indiceDia = esDomingo ? indice - partidosPorDia : indice;

                var fechaPartidoIdaLocal = fechaJornadaIdaLocal
                    .AddDays(esDomingo ? 1 : 0)
                    .AddHours(14 + 2 * indiceDia);

                var fechaPartidoVueltaLocal = fechaJornadaVueltaLocal
                    .AddDays(esDomingo ? 1 : 0)
                    .AddHours(14 + 2 * indiceDia);

                var fechaPartidoIdaUtc =
                    DateTime.SpecifyKind(fechaPartidoIdaLocal, DateTimeKind.Local).ToUniversalTime();
                var fechaPartidoVueltaUtc =
                    DateTime.SpecifyKind(fechaPartidoVueltaLocal, DateTimeKind.Local).ToUniversalTime();

                if (local != null && visitante != null)
                {
                    partidos.Add(new Partido
                    {
                        LigaId = ligaId,
                        Jornada = jornadaIda,
                        EquipoLocalId = local.Id,
                        EquipoVisitanteId = visitante.Id,
                        Estado = "Pendiente",
                        FechaHora = fechaPartidoIdaUtc
                    });
                    partidos.Add(new Partido
                    {
                        LigaId = ligaId,
                        Jornada = jornadaVuelta,
                        EquipoLocalId = visitante.Id,
                        EquipoVisitanteId = local.Id,
                        Estado = "Pendiente",
                        FechaHora = fechaPartidoVueltaUtc
                    });
                }
            }

            var ultimo = lista[n - 1];
            lista.RemoveAt(n - 1);
            lista.Insert(1, ultimo);
        }

        await _partidoRepository.CrearRangoAsync(partidos);

        int jornadasTotales = (n - 1) * 2;
        await _ligaRepository.ActualizarJornadasAsync(ligaId, jornadasTotales);

        return new
        {
            mensaje = "Calendario generado correctamente", jornadas = jornadasTotales, totalPartidos = partidos.Count
        };
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
                    clasificacion[local.Id] = new EquipoClasificacionResponseDto
                        { Id = local.Id, Nombre = local.Nombre };

                var s = clasificacion[local.Id];
                s.PartidosJugados++;
                s.GolesAFavor += partido.GolesLocal ?? 0;
                s.GolesEnContra += partido.GolesVisitante ?? 0;
                if (partido.GolesLocal > partido.GolesVisitante)
                {
                    s.Ganancias++;
                    s.Puntos += 3;
                }
                else if (partido.GolesLocal == partido.GolesVisitante)
                {
                    s.Empates++;
                    s.Puntos += 1;
                }
                else s.Derrotas++;
            }

            if (visitante != null)
            {
                if (!clasificacion.ContainsKey(visitante.Id))
                    clasificacion[visitante.Id] = new EquipoClasificacionResponseDto
                        { Id = visitante.Id, Nombre = visitante.Nombre };

                var s = clasificacion[visitante.Id];
                s.PartidosJugados++;
                s.GolesAFavor += partido.GolesVisitante ?? 0;
                s.GolesEnContra += partido.GolesLocal ?? 0;
                if (partido.GolesVisitante > partido.GolesLocal)
                {
                    s.Ganancias++;
                    s.Puntos += 3;
                }
                else if (partido.GolesVisitante == partido.GolesLocal)
                {
                    s.Empates++;
                    s.Puntos += 1;
                }
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