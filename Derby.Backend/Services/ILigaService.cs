using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Services;

public interface ILigaService
{
    Task<List<Equipo>> ObtenerEquiposAsync(int ligaId);
    Task<List<Equipo>> ObtenerEquiposSinLigaAsync();
    Task AgregarEquipoAsync(int ligaId, int equipoId);
    Task QuitarEquipoAsync(int ligaId, int equipoId);
    Task<object> GenerarCalendarioAsync(int ligaId);
    Task<List<JornadaResponseDto>> ObtenerJornadasAsync(int ligaId);
    Task<List<ResultadoPartidoResponseDto>> ObtenerResultadosAsync(int ligaId);
    Task<List<EquipoClasificacionResponseDto>> ObtenerClasificacionAsync(int ligaId);
}
