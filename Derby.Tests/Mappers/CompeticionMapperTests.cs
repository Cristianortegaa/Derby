using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Xunit;

namespace Derby.Tests.Mappers;

public class CompeticionMapperTests
{
    [Fact]
    public void ToDto_DeberiaMapearTodosLosCampos()
    {
        // ==================== ARRANGE ====================
        var comp = new Competicion
        {
            Id = 3, Nombre = "Liga Derby", Temporada = "2025/2026",
            Descripcion = "Competición principal", Estado = "Activo",
            TipoJuego = "Futbol-11", Grupo = "Único"
        };

        // ==================== ACT ====================
        var dto = comp.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(3, dto.Id);
        Assert.Equal("Liga Derby", dto.Nombre);
        Assert.Equal("2025/2026", dto.Temporada);
        Assert.Equal("Competición principal", dto.Descripcion);
        Assert.Equal("Activo", dto.Estado);
        Assert.Equal("Futbol-11", dto.TipoJuego);
        Assert.Equal("Único", dto.Grupo);
    }

    [Fact]
    public void ToDto_CuandoCamposNullablesSonNulos_DeberianSerNullEnElDto()
    {
        // ==================== ARRANGE ====================
        var comp = new Competicion
        {
            Id = 1, Nombre = "X", Temporada = "T", Estado = "Activo",
            Descripcion = null, TipoJuego = null, Grupo = null
        };

        // ==================== ACT ====================
        var dto = comp.ToDto();

        // ==================== ASSERT ====================
        Assert.Null(dto.Descripcion);
        Assert.Null(dto.TipoJuego);
        Assert.Null(dto.Grupo);
    }
}
