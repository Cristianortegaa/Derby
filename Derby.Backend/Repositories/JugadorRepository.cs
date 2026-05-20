using Derby.Backend.Models;
using Derby.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Repositories;

public class JugadorRepository : IJugadorRepository
{
    private readonly DerbyContext _context;

    public JugadorRepository(DerbyContext context)
    {
        _context = context;
    }

    public async Task<List<Jugador>> ObtenerPorEquipoAsync(int equipoId)
    {
        return await _context.Jugadores.Where(j => j.EquipoId == equipoId).ToListAsync();
    }

    public async Task<Jugador?> ObtenerPorIdAsync(int id)
    {
        return await _context.Jugadores.FindAsync(id);
    }

    public async Task AgregarAsync(Jugador jugador)
    {
        _context.Jugadores.Add(jugador);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Jugador jugador)
    {
        _context.Jugadores.Update(jugador);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Jugador jugador)
    {
        _context.Jugadores.Remove(jugador);
        await _context.SaveChangesAsync();
    }
}