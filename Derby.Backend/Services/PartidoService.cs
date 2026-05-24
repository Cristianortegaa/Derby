using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class PartidoService : IPartidoService
{
    private readonly IPartidoRepository _partidoRepository;

    public PartidoService(IPartidoRepository partidoRepository)
    {
        _partidoRepository = partidoRepository;
    }

    public async Task<List<PartidoResponseDto>> ObtenerTodosAsync()
    {
        var partidos = await _partidoRepository.ObtenerTodosAsync();
        return partidos.Select(p => p.ToDto()).ToList();
    }

    public async Task<PartidoResponseDto?> ObtenerPorIdAsync(int id)
    {
        var partido = await _partidoRepository.ObtenerPorIdAsync(id);
        return partido?.ToDto();
    }

    public async Task<PartidoResponseDto> CrearAsync(PartidoRequestDto dto)
    {
        var partido = new Partido
        {
            Jornada = dto.Jornada,
            LigaId = dto.LigaId,
            EquipoLocalId = dto.EquipoLocalId,
            EquipoVisitanteId = dto.EquipoVisitanteId,
            GolesLocal = dto.GolesLocal,
            GolesVisitante = dto.GolesVisitante,
            Estado = dto.Estado,
            FechaHora = dto.FechaHora,
            ArbitroId = dto.ArbitroId
        };
        var creado = await _partidoRepository.CrearAsync(partido);
        return creado.ToDto();
    }

    public async Task<PartidoResponseDto?> ActualizarAsync(int id, PartidoRequestDto dto)
    {
        var partido = new Partido
        {
            Jornada = dto.Jornada,
            LigaId = dto.LigaId,
            EquipoLocalId = dto.EquipoLocalId,
            EquipoVisitanteId = dto.EquipoVisitanteId,
            GolesLocal = dto.GolesLocal,
            GolesVisitante = dto.GolesVisitante,
            Estado = dto.Estado,
            FechaHora = dto.FechaHora,
            ArbitroId = dto.ArbitroId
        };
        var actualizado = await _partidoRepository.ActualizarAsync(id, partido);
        return actualizado?.ToDto();
    }

    public async Task<bool> EliminarAsync(int id)
    {
        return await _partidoRepository.EliminarAsync(id);
    }
}
