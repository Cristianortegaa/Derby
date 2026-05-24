using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Repositories;

public class LigaRepository : ILigaRepository
{
    private readonly DerbyContext _context;

    public LigaRepository(DerbyContext context)
    {
        _context = context;
    }

    public async Task<Liga?> ObtenerPorIdAsync(int ligaId)
    {
        return await _context.Ligas.FindAsync(ligaId);
    }

    public async Task<List<Equipo>> ObtenerEquiposAsync(int ligaId)
    {
        return await _context.LigaEquipos
            .Where(le => le.LigaId == ligaId)
            .Include(le => le.Equipo)
            .Select(le => le.Equipo!)
            .ToListAsync();
    }

    public async Task<List<Equipo>> ObtenerEquiposSinLigaAsync()
    {
        var equiposConLiga = await _context.LigaEquipos
            .Select(le => le.EquipoId)
            .ToListAsync();

        return await _context.Equipos
            .Where(e => !equiposConLiga.Contains(e.Id))
            .ToListAsync();
    }

    public async Task<bool> EquipoExisteAsync(int ligaId, int equipoId)
    {
        return await _context.LigaEquipos
            .AnyAsync(le => le.LigaId == ligaId && le.EquipoId == equipoId);
    }

    public async Task AgregarEquipoAsync(int ligaId, int equipoId)
    {
        _context.LigaEquipos.Add(new LigaEquipo { LigaId = ligaId, EquipoId = equipoId });
        await _context.SaveChangesAsync();
    }

    public async Task QuitarEquipoAsync(int ligaId, int equipoId)
    {
        var le = await _context.LigaEquipos
            .FirstOrDefaultAsync(le => le.LigaId == ligaId && le.EquipoId == equipoId);
        if (le != null)
        {
            _context.LigaEquipos.Remove(le);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ActualizarJornadasAsync(int ligaId, int jornadas)
    {
        var liga = await _context.Ligas.FindAsync(ligaId);
        if (liga != null)
        {
            liga.Jornadas = jornadas;
            liga.JornadaActual = 1;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> TienePartidosAsync(int ligaId)
    {
        return await _context.Partidos.AnyAsync(p => p.LigaId == ligaId);
    }

    public async Task<List<Liga>> ObtenerTodasAsync()
    {
        return await _context.Ligas
            .Include(l => l.Partidos)
            .ToListAsync();
    }

    public async Task<Liga> CrearAsync(Liga liga)
    {
        _context.Ligas.Add(liga);
        await _context.SaveChangesAsync();
        return liga;
    }

    public async Task<Liga?> ActualizarAsync(int id, Liga liga)
    {
        var existing = await _context.Ligas.FindAsync(id);
        if (existing == null)
            return null;

        existing.Nombre = liga.Nombre;
        existing.CompeticionId = liga.CompeticionId;
        existing.Grupo = liga.Grupo;
        existing.Jornadas = liga.Jornadas;
        existing.JornadaActual = liga.JornadaActual;
        existing.Estado = liga.Estado;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var liga = await _context.Ligas.FindAsync(id);
        if (liga == null)
            return false;

        _context.Ligas.Remove(liga);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<LigaEquipo>> ObtenerTodasAsignacionesAsync()
    {
        return await _context.LigaEquipos
            .Include(le => le.Liga)
            .ToListAsync();
    }
}

