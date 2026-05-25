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
            .Include(p => p.Arbitro)
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
        existing.ArbitroId = partido.ArbitroId;

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
    
    public async Task EliminarPorLigaAsync(int ligaId)
    {
        var partidos = await _context.Partidos.Where(p => p.LigaId == ligaId).ToListAsync();
        _context.Partidos.RemoveRange(partidos);
        await _context.SaveChangesAsync();
    }

    public async Task CrearRangoAsync(List<Partido> partidos)
    {
        _context.Partidos.AddRange(partidos);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Partido>> ObtenerTodosAsync()
    {
        return await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Include(p => p.Liga)
            .Include(p => p.Arbitro)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Partido>> ObtenerFinalizadosAsync()
    {
        return await _context.Partidos
            .Where(p => p.Estado == "Finalizado")
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Include(p => p.Liga)
            .Include(p => p.Arbitro)
            .OrderByDescending(p => p.FechaHora)
            .ToListAsync();
    }

    public async Task<Partido?> ActualizarGolesAsync(int id, int? golesLocal, int? golesVisitante)
    {
        var partido = await _context.Partidos.FindAsync(id);
        if (partido == null) return null;
        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        await _context.SaveChangesAsync();
        return partido;
    }

    public async Task<Partido?> FinalizarAsync(int partidoId, int golesLocal, int golesVisitante)
    {
        var partido = await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .FirstOrDefaultAsync(p => p.Id == partidoId);

        if (partido == null)
            return null;

        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        partido.Estado = "Finalizado";

        await _context.SaveChangesAsync();
        return partido;
    }
}
