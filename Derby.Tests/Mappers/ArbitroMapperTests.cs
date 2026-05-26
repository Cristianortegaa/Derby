using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Xunit;

namespace Derby.Tests.Mappers;

public class ArbitroMapperTests
{
    [Fact]
    public void ToDto_DeberiaMapearTodosLosCamposDelModelo()
    {
        // ==================== ARRANGE ====================
        var arbitro = new Arbitro { Id = 42, Nombre = "Roberto", Apellidos = "Fernández", NumeroColegiado = "COL-123" };

        // ==================== ACT ====================
        var dto = arbitro.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(42, dto.Id);
        Assert.Equal("Roberto", dto.Nombre);
        Assert.Equal("Fernández", dto.Apellidos);
        Assert.Equal("COL-123", dto.NumeroColegiado);
    }

    [Fact]
    public void ToDto_ConCamposVacios_NoDeberiaLanzarExcepcion()
    {
        // ==================== ARRANGE ====================
        var arbitro = new Arbitro { Id = 0, Nombre = "", Apellidos = "", NumeroColegiado = "" };

        // ==================== ACT ====================
        var dto = arbitro.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal("", dto.Nombre);
        Assert.Equal("", dto.Apellidos);
        Assert.Equal("", dto.NumeroColegiado);
    }

    [Fact]
    public void ToEntity_DeberiaMapearLosCamposDesdeDto()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "Ana", Apellidos = "Torres", NumeroColegiado = "COL-999" };

        // ==================== ACT ====================
        var entidad = dto.ToEntity();

        // ==================== ASSERT ====================
        Assert.Equal("Ana", entidad.Nombre);
        Assert.Equal("Torres", entidad.Apellidos);
        Assert.Equal("COL-999", entidad.NumeroColegiado);
    }

    [Fact]
    public void ToEntity_NoDeberiaAsignarId_DeberiaQuedarEnCero()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "X", Apellidos = "Y", NumeroColegiado = "Z" };

        // ==================== ACT ====================
        var entidad = dto.ToEntity();

        // ==================== ASSERT ====================
        Assert.Equal(0, entidad.Id);
    }

    [Fact]
    public void RoundTrip_ToEntityLuegoToDto_DeberiaMantienerLosValores()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "Luisa", Apellidos = "Gómez", NumeroColegiado = "X-55" };

        // ==================== ACT ====================
        var entidad = dto.ToEntity();
        entidad.Id  = 7;
        var resultado = entidad.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(dto.Nombre, resultado.Nombre);
        Assert.Equal(dto.Apellidos, resultado.Apellidos);
        Assert.Equal(dto.NumeroColegiado, resultado.NumeroColegiado);
    }
}
