using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface ILigaRepository
{
    Task<Liga?> ObtenerPorIdAsync(int ligaId);
    Task<List<Equipo>> ObtenerEquiposAsync(int ligaId);
    Task<List<Equipo>> ObtenerEquiposSinLigaAsync();
    Task<bool> EquipoExisteAsync(int ligaId, int equipoId);
    Task AgregarEquipoAsync(int ligaId, int equipoId);
    Task QuitarEquipoAsync(int ligaId, int equipoId);
    Task ActualizarJornadasAsync(int ligaId, int jornadas);
    Task<bool> TienePartidosAsync(int ligaId);
    Task<List<Liga>> ObtenerTodasAsync();
    Task<Liga> CrearAsync(Liga liga);
    Task<Liga?> ActualizarAsync(int id, Liga liga);
    Task<bool> EliminarAsync(int id);
    Task<List<LigaEquipo>> ObtenerTodasAsignacionesAsync();
}
