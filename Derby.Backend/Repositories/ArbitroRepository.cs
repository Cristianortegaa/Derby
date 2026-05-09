using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Derby.Backend.Data;
using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public class ArbitroRepository : IArbitroRepository
{
    private readonly DerbyContext _context;
    private readonly ILogger<ArbitroRepository> _logger;

    public ArbitroRepository(DerbyContext context, ILogger<ArbitroRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Arbitro>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Consultando todos los árbitros en la base de datos.");
        return await _context.Arbitros.ToListAsync();
    }

    public async Task<Arbitro?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Consultando el árbitro con ID: {Id} en la base de datos.", id);
        return await _context.Arbitros.FindAsync(id);
    }

    public async Task<Arbitro> CrearAsync(Arbitro arbitro)
    {
        _logger.LogInformation("Insertando un nuevo árbitro en la base de datos.");
        await _context.Arbitros.AddAsync(arbitro);
        await _context.SaveChangesAsync();
        return arbitro;
    }

    public async Task<Arbitro> ActualizarAsync(Arbitro arbitro)
    {
        _logger.LogInformation("Actualizando el árbitro con ID: {Id} en la base de datos.", arbitro.Id);
        _context.Arbitros.Update(arbitro);
        await _context.SaveChangesAsync();
        return arbitro;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        _logger.LogInformation("Iniciando el proceso de eliminación para el árbitro con ID: {Id}.", id);

        var arbitro = await _context.Arbitros.FindAsync(id);

        if (arbitro == null)
        {
            _logger.LogInformation("El árbitro con ID: {Id} no existe en la base de datos. Se cancela la eliminación.", id);
            return false;
        }

        _logger.LogInformation("Eliminando definitivamente el árbitro con ID: {Id} de la base de datos.", id);
        _context.Arbitros.Remove(arbitro);
        await _context.SaveChangesAsync();

        return true;
    }
}

