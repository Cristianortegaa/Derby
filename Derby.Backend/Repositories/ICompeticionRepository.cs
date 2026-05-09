using Derby.Backend.Models;

namespace Derby.Backend.Repositories;

public interface ICompeticionRepository
{
    Task<List<Competicion>> ObtenerTodasAsync();
    Task<Competicion?> ObtenerPorIdAsync(int id);
    Task<Competicion?> ObtenerPorNombreYTemporadaAsync(string nombre, string temporada);
    Task<List<Competicion>> FiltrarAsync(string? temporada = null, string? tipoJuego = null, string? competicion = null, string? grupo = null);
    Task<Competicion> CrearAsync(Competicion competicion);
    Task<Competicion?> ActualizarAsync(int id, Competicion competicion);
    Task<bool> EliminarAsync(int id);
}

