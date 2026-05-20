using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class JugadorMapper
{
    public static JugadorResponseDto ToDto(this Jugador jugador)
    {
        return new JugadorResponseDto
        {
            Id = jugador.Id,
            Nombre = jugador.Nombre,
            Dorsal = jugador.Dorsal,
            EquipoId = jugador.EquipoId
        };
    }

    public static Jugador ToEntity(this JugadorRequestDto dto, int equipoId)
    {
        return new Jugador
        {
            Nombre = dto.Nombre,
            Dorsal = dto.Dorsal,
            EquipoId = equipoId
        };
    }
}