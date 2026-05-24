using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Services;

public interface ICompeticionService
{
    Task<List<CompeticionResponseDto>> ObtenerTodasAsync();
    Task<CompeticionResponseDto?> ObtenerPorIdAsync(int id);
    Task<CompeticionResponseDto> CrearAsync(Competicion competicion);
    Task<CompeticionResponseDto?> ActualizarAsync(int id, Competicion competicion);
    Task<bool> EliminarAsync(int id);
    Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int competicionId);
    Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int competicionId);
    Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int competicionId);
    Task<List<GoleadorResponseDto>> ObtenerGoleadoresAsync(int competicionId);
    Task<List<Competicion>> BuscarCompeticionesAsync(string? temporada, string? tipoJuego, string? competicion, string? grupo);
}
