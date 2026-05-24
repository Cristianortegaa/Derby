using Derby.Backend.Dtos;

namespace Derby.Backend.Services;

public interface IPartidoService
{
    Task<List<PartidoResponseDto>> ObtenerTodosAsync();
    Task<PartidoResponseDto?> ObtenerPorIdAsync(int id);
    Task<PartidoResponseDto> CrearAsync(PartidoRequestDto dto);
    Task<PartidoResponseDto?> ActualizarAsync(int id, PartidoRequestDto dto);
    Task<bool> EliminarAsync(int id);
}
