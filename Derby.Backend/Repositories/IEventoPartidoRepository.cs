using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface IEventoPartidoRepository
{
    Task<List<EventoPartido>> ObtenerPorPartidoAsync(int partidoId);
    Task<EventoPartido> CrearAsync(EventoPartido evento);
    Task<bool> EliminarAsync(int id);
    Task<List<EventoPartido>> ObtenerGolesPorLigaAsync(int ligaId);
    Task<List<EventoPartido>> ObtenerGolesPorCompeticionAsync(int competicionId);
}
