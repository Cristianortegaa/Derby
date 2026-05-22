using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class EventoPartidoMapper
{
    public static EventoPartidoResponseDto ToDto(this EventoPartido evento)
    {
        return new EventoPartidoResponseDto
        {
            Id = evento.Id,
            Minuto = evento.Minuto,
            TipoEvento = evento.TipoEvento.ToString(),
            JugadorId = evento.JugadorId,
            NombreJugador = evento.Jugador?.Nombre ?? "Desconocido",
            PartidoId = evento.PartidoId
        };
    }
}
