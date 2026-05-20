using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface IPartidoRepository
{
    Task<List<Partido>> ObtenerPorCompeticionAsync(int competicionId);
    Task<List<Partido>> ObtenerPorLigaAsync(int ligaId);
    Task<List<Partido>> ObtenerResultadosPorLigaAsync(int ligaId);
    Task<List<Partido>> ObtenerJornadaAsync(int competicionId, int jornadaNumero);
    Task<List<Partido>> ObtenerResultadosAsync(int competicionId);
    Task<Partido?> ObtenerPorIdAsync(int id);
    Task<Partido> CrearAsync(Partido partido);
    Task<Partido?> ActualizarAsync(int id, Partido partido);
    Task<bool> EliminarAsync(int id);
    Task CrearRangoAsync(List<Partido> partidos);
}

