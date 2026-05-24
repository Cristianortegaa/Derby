using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class EventoPartidoService : IEventoPartidoService
{
    private readonly IEventoPartidoRepository _eventoRepository;
    private readonly IPartidoRepository _partidoRepository;

    public EventoPartidoService(IEventoPartidoRepository eventoRepository, IPartidoRepository partidoRepository)
    {
        _eventoRepository = eventoRepository;
        _partidoRepository = partidoRepository;
    }

    public async Task<List<EventoPartidoResponseDto>> ObtenerEventosAsync(int partidoId)
    {
        var eventos = await _eventoRepository.ObtenerPorPartidoAsync(partidoId);
        return eventos.Select(e => e.ToDto()).ToList();
    }

    public async Task<EventoPartidoResponseDto?> AñadirEventoAsync(int partidoId, EventoPartidoRequestDto dto)
    {
        if (!Enum.TryParse<TipoEvento>(dto.TipoEvento, out var tipo))
            return null;

        var evento = new EventoPartido
        {
            PartidoId = partidoId,
            JugadorId = dto.JugadorId,
            Minuto = dto.Minuto,
            TipoEvento = tipo
        };

        var creado = await _eventoRepository.CrearAsync(evento);
        return creado.ToDto();
    }

    public async Task<bool> EliminarEventoAsync(int eventoId)
    {
        return await _eventoRepository.EliminarAsync(eventoId);
    }

    public async Task<Partido?> CerrarActaAsync(int partidoId)
    {
        var partido = await _partidoRepository.ObtenerPorIdAsync(partidoId);
        if (partido == null)
            return null;

        var eventos = await _eventoRepository.ObtenerPorPartidoAsync(partidoId);

        var golesLocal = eventos.Count(e =>
            e.TipoEvento == TipoEvento.Gol &&
            e.Jugador != null &&
            e.Jugador.EquipoId == partido.EquipoLocalId);

        var golesVisitante = eventos.Count(e =>
            e.TipoEvento == TipoEvento.Gol &&
            e.Jugador != null &&
            e.Jugador.EquipoId == partido.EquipoVisitanteId);

        return await _partidoRepository.FinalizarAsync(partidoId, golesLocal, golesVisitante);
    }
}
