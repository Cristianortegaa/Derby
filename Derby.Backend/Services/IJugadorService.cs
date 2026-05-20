using Derby.Backend.Dtos;

namespace Derby.Backend.Services;

public interface IJugadorService
{
    Task<List<JugadorResponseDto>> ObtenerPorEquipoAsync(int equipoId);
    Task AgregarAsync(int equipoId, JugadorRequestDto dto);
    Task ActualizarAsync(int id, JugadorRequestDto dto);
    Task EliminarAsync(int id);
}