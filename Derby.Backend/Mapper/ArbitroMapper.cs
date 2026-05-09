using Derby.Backend.Dtos;
using Derby.Backend.Models;

namespace Derby.Backend.Mappers;

public static class ArbitroMapper
{
    public static ArbitroResponseDto ToDto(this Arbitro arbitro)
    {
        return new ArbitroResponseDto
        {
            Id = arbitro.Id,
            Nombre = arbitro.Nombre,
            Apellidos = arbitro.Apellidos,
            NumeroColegiado = arbitro.NumeroColegiado
        };
    }

    public static Arbitro ToEntity(this ArbitroRequestDto dto)
    {
        return new Arbitro
        {
            Nombre = dto.Nombre,
            Apellidos = dto.Apellidos,
            NumeroColegiado = dto.NumeroColegiado
        };
    }
}

