using Derby.Backend.Data;
using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Services;

public interface IEventoPartidoService
{
    Task<List<EventoPartidoResponseDto>> ObtenerEventosAsync(int partidoId);
    Task<EventoPartidoResponseDto?> AñadirEventoAsync(int partidoId, EventoPartidoRequestDto dto);
    Task<bool> EliminarEventoAsync(int eventoId);
    Task<Partido?> CerrarActaAsync(int partidoId);
}

public class EventoPartidoService : IEventoPartidoService
{
    private readonly IEventoPartidoRepository _eventoRepository;
    private readonly DerbyContext _context;

    public EventoPartidoService(IEventoPartidoRepository eventoRepository, DerbyContext context)
    {
        _eventoRepository = eventoRepository;
        _context = context;
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
        var partido = await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .FirstOrDefaultAsync(p => p.Id == partidoId);

        if (partido == null)
            return null;

        var eventos = await _eventoRepository.ObtenerPorPartidoAsync(partidoId);

        partido.GolesLocal = eventos.Count(e =>
            e.TipoEvento == TipoEvento.Gol &&
            e.Jugador != null &&
            e.Jugador.EquipoId == partido.EquipoLocalId);

        partido.GolesVisitante = eventos.Count(e =>
            e.TipoEvento == TipoEvento.Gol &&
            e.Jugador != null &&
            e.Jugador.EquipoId == partido.EquipoVisitanteId);

        partido.Estado = "Finalizado";

        await _context.SaveChangesAsync();
        return partido;
    }
}
