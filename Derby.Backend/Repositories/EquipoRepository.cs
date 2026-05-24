using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using Derby.Backend.Data;
using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public class EquipoRepository : IEquipoRepository
{
    private readonly DerbyContext _context;
    private readonly ILogger<EquipoRepository> _logger; 
    
    public EquipoRepository(DerbyContext context, ILogger<EquipoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Equipo>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando todos los equipos en la base de datos.");
        return await _context.Equipos.ToListAsync();
    }

    public async Task<Equipo?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Consultando el equipo con ID: {Id} en la base de datos.", id);
        return await _context.Equipos.FindAsync(id);
    }
    
    public async Task<Equipo> CrearAsync(Equipo equipo)
    {
        _logger.LogInformation("Insertando un nuevo equipo en la base de datos.");
        await _context.Equipos.AddAsync(equipo);
        await _context.SaveChangesAsync();
        return equipo;
    }

    public async Task<Equipo> ActualizarAsync(Equipo equipo)
    {
        _logger.LogInformation("Actualizando el equipo con ID: {Id} en la base de datos.", equipo.Id);
        _context.Equipos.Update(equipo);
        await _context.SaveChangesAsync();
        return equipo;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        _logger.LogInformation("Iniciando el proceso de eliminación para el equipo con ID: {Id}.", id);
        
        var equipo = await _context.Equipos.FindAsync(id);
        
        if (equipo == null) 
        {
            _logger.LogInformation("El equipo con ID: {Id} no existe en la base de datos. Se cancela la eliminación.", id);
            return false; 
        }

        _logger.LogInformation("Eliminando definitivamente el equipo con ID: {Id} de la base de datos.", id);
        _context.Equipos.Remove(equipo);
        await _context.SaveChangesAsync();
        
        return true;
    }
}