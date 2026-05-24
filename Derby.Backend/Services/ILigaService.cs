using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Services;

public interface ILigaService
{
    Task<List<LigaResponseDto>> ObtenerTodasAsync();
    Task<LigaResponseDto?> ObtenerPorIdAsync(int id);
    Task<LigaResponseDto> CrearAsync(LigaRequestDto dto);
    Task<LigaResponseDto?> ActualizarAsync(int id, LigaRequestDto dto);
    Task<bool> EliminarAsync(int id);
    Task<List<Equipo>> ObtenerEquiposAsync(int ligaId);
    Task<List<Equipo>> ObtenerEquiposSinLigaAsync();
    Task AgregarEquipoAsync(int ligaId, int equipoId);
    Task QuitarEquipoAsync(int ligaId, int equipoId);
    Task<object> GenerarCalendarioAsync(int ligaId);
    Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int ligaId);
    Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int ligaId);
    Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int ligaId);
    Task<List<GoleadorResponseDto>> ObtenerGoleadoresAsync(int ligaId);
}
