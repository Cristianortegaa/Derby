using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Repositories;

public class CompeticionRepository : ICompeticionRepository
{
    private readonly DerbyContext _context;

    public CompeticionRepository(DerbyContext context)
    {
        _context = context;
    }

    public async Task<List<Competicion>> ObtenerTodasAsync()
        => await _context.Competiciones.ToListAsync();

    public async Task<Competicion?> ObtenerPorIdAsync(int id)
        => await _context.Competiciones.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Competicion?> ObtenerPorNombreYTemporadaAsync(string nombre, string temporada)
        => await _context.Competiciones
            .FirstOrDefaultAsync(c => c.Nombre == nombre && c.Temporada == temporada);

    public async Task<List<Competicion>> FiltrarAsync(string? temporada = null, string? tipoJuego = null, string? competicion = null, string? grupo = null)
    {
        var query = _context.Competiciones.AsQueryable();

        if (!string.IsNullOrEmpty(temporada))
            query = query.Where(c => c.Temporada == temporada);

        if (!string.IsNullOrEmpty(tipoJuego))
            query = query.Where(c => c.TipoJuego == tipoJuego);

        if (!string.IsNullOrEmpty(competicion))
            query = query.Where(c => c.Nombre.Contains(competicion));

        if (!string.IsNullOrEmpty(grupo))
            query = query.Where(c => c.Grupo == grupo);

        return await query.ToListAsync();
    }

    public async Task<Competicion> CrearAsync(Competicion competicion)
    {
        _context.Competiciones.Add(competicion);
        await _context.SaveChangesAsync();
        return competicion;
    }

    public async Task<Competicion?> ActualizarAsync(int id, Competicion competicion)
    {
        var existing = await _context.Competiciones.FindAsync(id);
        if (existing == null)
            return null;

        existing.Nombre = competicion.Nombre;
        existing.Temporada = competicion.Temporada;
        existing.Descripcion = competicion.Descripcion;
        existing.Estado = competicion.Estado;
        existing.TipoJuego = competicion.TipoJuego;
        existing.Grupo = competicion.Grupo;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var competicion = await _context.Competiciones.FindAsync(id);
        if (competicion == null)
            return false;

        _context.Competiciones.Remove(competicion);
        await _context.SaveChangesAsync();
        return true;
    }
}
