using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Repositories;

public class EventoPartidoRepository : IEventoPartidoRepository
{
    private readonly DerbyContext _context;

    public EventoPartidoRepository(DerbyContext context)
    {
        _context = context;
    }

    public async Task<List<EventoPartido>> ObtenerPorPartidoAsync(int partidoId)
    {
        return await _context.EventosPartidos
            .Where(e => e.PartidoId == partidoId)
            .Include(e => e.Jugador)
            .OrderBy(e => e.Minuto)
            .ToListAsync();
    }

    public async Task<EventoPartido> CrearAsync(EventoPartido evento)
    {
        _context.EventosPartidos.Add(evento);
        await _context.SaveChangesAsync();
        return evento;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await _context.EventosPartidos.FindAsync(id);
        if (evento == null) return false;

        _context.EventosPartidos.Remove(evento);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<EventoPartido>> ObtenerGolesPorLigaAsync(int ligaId)
    {
        return await _context.EventosPartidos
            .Where(e => e.TipoEvento == TipoEvento.Gol && e.Partido != null && e.Partido.LigaId == ligaId)
            .Include(e => e.Jugador)
                .ThenInclude(j => j!.Equipo)
            .Include(e => e.Partido)
            .ToListAsync();
    }

    public async Task<List<EventoPartido>> ObtenerGolesPorCompeticionAsync(int competicionId)
    {
        return await _context.EventosPartidos
            .Where(e => e.TipoEvento == TipoEvento.Gol && e.Partido != null && e.Partido.Liga != null && e.Partido.Liga.CompeticionId == competicionId)
            .Include(e => e.Jugador)
                .ThenInclude(j => j!.Equipo)
            .Include(e => e.Partido)
                .ThenInclude(p => p!.Liga)
            .ToListAsync();
    }
}
