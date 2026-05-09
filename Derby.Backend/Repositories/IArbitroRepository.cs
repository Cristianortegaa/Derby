using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface IArbitroRepository
{
    Task<IEnumerable<Arbitro>> ObtenerTodosAsync();
    Task<Arbitro?> ObtenerPorIdAsync(int id);
    Task<Arbitro> CrearAsync(Arbitro arbitro);
    Task<Arbitro> ActualizarAsync(Arbitro arbitro);
    Task<bool> EliminarAsync(int id);
}

