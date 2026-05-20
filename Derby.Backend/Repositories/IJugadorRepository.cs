using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface IJugadorRepository
{
    Task<List<Jugador>> ObtenerPorEquipoAsync(int equipoId);
    Task<Jugador?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Jugador jugador);
    Task ActualizarAsync(Jugador jugador);
    Task EliminarAsync(Jugador jugador);
}