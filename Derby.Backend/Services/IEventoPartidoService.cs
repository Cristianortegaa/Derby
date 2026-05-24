using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Services;

public interface IEventoPartidoService
{
    Task<List<EventoPartidoResponseDto>> ObtenerEventosAsync(int partidoId);
    Task<EventoPartidoResponseDto?> AñadirEventoAsync(int partidoId, EventoPartidoRequestDto dto);
    Task<bool> EliminarEventoAsync(int eventoId);
    Task<Partido?> CerrarActaAsync(int partidoId);
}
