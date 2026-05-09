using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface IEquipoRepository
{
    Task<IEnumerable<Equipo>> ObtenerTodosAsync();
    
    Task<Equipo?> ObtenerPorIdAsync(int id);
    
    Task<Equipo> CrearAsync(Equipo equipo);

    Task<Equipo> ActualizarAsync(Equipo equipo);

    Task<bool> EliminarAsync(int id);
}