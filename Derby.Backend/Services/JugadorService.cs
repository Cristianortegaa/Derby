using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class JugadorService : IJugadorService
{
    private readonly IJugadorRepository _jugadorRepository;

    public JugadorService(IJugadorRepository jugadorRepository)
    {
        _jugadorRepository = jugadorRepository;
    }

    public async Task<List<JugadorResponseDto>> ObtenerPorEquipoAsync(int equipoId)
    {
        var jugadores = await _jugadorRepository.ObtenerPorEquipoAsync(equipoId);
        return jugadores.Select(j => j.ToDto()).ToList();
    }

    public async Task AgregarAsync(int equipoId, JugadorRequestDto dto)
    {
        var jugadores = await _jugadorRepository.ObtenerPorEquipoAsync(equipoId);

        if (jugadores.Count >= 25)
            throw new Exception("El equipo ya tiene el máximo de 25 jugadores");

        if (jugadores.Any(j => j.Dorsal == dto.Dorsal))
            throw new Exception("Ya existe un jugador con ese dorsal en el equipo");

        var jugador = dto.ToEntity(equipoId);
        await _jugadorRepository.AgregarAsync(jugador);
    }

    public async Task ActualizarAsync(int id, JugadorRequestDto dto)
    {
        var jugador = await _jugadorRepository.ObtenerPorIdAsync(id)
                      ?? throw new Exception("Jugador no encontrado");

        var jugadores = await _jugadorRepository.ObtenerPorEquipoAsync(jugador.EquipoId);

        if (jugadores.Any(j => j.Dorsal == dto.Dorsal && j.Id != id))
            throw new Exception("Ya existe un jugador con ese dorsal en el equipo");

        jugador.Nombre = dto.Nombre;
        jugador.Dorsal = dto.Dorsal;
        await _jugadorRepository.ActualizarAsync(jugador);
    }

    public async Task EliminarAsync(int id)
    {
        var jugador = await _jugadorRepository.ObtenerPorIdAsync(id)
                      ?? throw new Exception("Jugador no encontrado");

        await _jugadorRepository.EliminarAsync(jugador);
    }
}