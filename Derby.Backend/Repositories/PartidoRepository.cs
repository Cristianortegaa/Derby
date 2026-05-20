using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Repositories;

public class PartidoRepository : IPartidoRepository
{
    private readonly DerbyContext _context;

    public PartidoRepository(DerbyContext context)
    {
        _context = context;
    }

    public async Task<List<Partido>> ObtenerPorCompeticionAsync(int competicionId)
    {
        return await _context.Partidos
            .Where(p => p.Liga != null && p.Liga.CompeticionId == competicionId)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Partido>> ObtenerPorLigaAsync(int ligaId)
    {
        return await _context.Partidos
            .Where(p => p.LigaId == ligaId)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Partido>> ObtenerResultadosPorLigaAsync(int ligaId)
    {
        return await _context.Partidos
            .Where(p => p.LigaId == ligaId && p.Estado == "Finalizado")
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderByDescending(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Partido>> ObtenerJornadaAsync(int competicionId, int jornadaNumero)
    {
        return await _context.Partidos
            .Where(p => p.Liga != null && p.Liga.CompeticionId == competicionId && p.Jornada == jornadaNumero)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Partido>> ObtenerResultadosAsync(int competicionId)
    {
        return await _context.Partidos
            .Where(p => p.Liga != null && p.Liga.CompeticionId == competicionId && p.Estado == "Finalizado")
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderByDescending(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<Partido?> ObtenerPorIdAsync(int id)
    {
        return await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Partido> CrearAsync(Partido partido)
    {
        _context.Partidos.Add(partido);
        await _context.SaveChangesAsync();
        return partido;
    }

    public async Task<Partido?> ActualizarAsync(int id, Partido partido)
    {
        var existing = await _context.Partidos.FindAsync(id);
        if (existing == null)
            return null;

        existing.FechaHora = partido.FechaHora;
        existing.GolesLocal = partido.GolesLocal;
        existing.GolesVisitante = partido.GolesVisitante;
        existing.Estado = partido.Estado;
        existing.EquipoLocalId = partido.EquipoLocalId;
        existing.EquipoVisitanteId = partido.EquipoVisitanteId;
        existing.Jornada = partido.Jornada;
        existing.LigaId = partido.LigaId;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var partido = await _context.Partidos.FindAsync(id);
        if (partido == null)
            return false;

        _context.Partidos.Remove(partido);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task CrearRangoAsync(List<Partido> partidos)
    {
        _context.Partidos.AddRange(partidos);
        await _context.SaveChangesAsync();
    }
}
