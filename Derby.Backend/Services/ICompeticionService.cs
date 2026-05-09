using Derby.Backend.Dtos;

namespace Derby.Backend.Services;

public interface ICompeticionService
{
    Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int competicionId);
    Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int competicionId);
    Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int competicionId);
    Task<List<GoleadorResponseDto>> ObtenerGoleadoresAsync(int competicionId);
    Task<List<JornadaResponseDto>> BuscarCompeticionesAsync(string? temporada, string? tipoJuego, string? competicion, string? grupo);
}

