using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Xunit;

namespace Derby.Tests.Mappers;

public class EquipoMapperTests
{
    [Fact]
    public void ToDto_DeberiaMapearTodosLosCamposIncluidaLiga()
    {
        // ==================== ARRANGE ====================
        var equipo = new Equipo
        {
            Id = 10, Nombre = "FC Derby Norte", EscudoUrl = "https://cdn.derby.com/norte.png",
            Sede = "Estadio Norte", Entrenador = "Marcos Gil", LigaNombre = "Primera DAW"
        };

        // ==================== ACT ====================
        var dto = equipo.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(10, dto.Id);
        Assert.Equal("FC Derby Norte", dto.Nombre);
        Assert.Equal("https://cdn.derby.com/norte.png", dto.EscudoUrl);
        Assert.Equal("Estadio Norte", dto.Sede);
        Assert.Equal("Marcos Gil", dto.Entrenador);
        Assert.Equal("Primera DAW", dto.LigaNombre);
    }

    [Fact]
    public void ToDto_SinLigaAsignada_LigaNombreDeberiaSerNull()
    {
        // ==================== ARRANGE ====================
        var equipo = new Equipo { Id = 1, Nombre = "Sin Liga", EscudoUrl = "", Sede = "", Entrenador = "", LigaNombre = null };

        // ==================== ACT ====================
        var dto = equipo.ToDto();

        // ==================== ASSERT ====================
        Assert.Null(dto.LigaNombre);
    }

    [Fact]
    public void ToEntity_DeberiaMapearLosCamposDesdeDto()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "UD Miralba", EscudoUrl = "url", Sede = "Campo Miralba", Entrenador = "Sandra Vega" };

        // ==================== ACT ====================
        var entidad = dto.ToEntity();

        // ==================== ASSERT ====================
        Assert.Equal("UD Miralba", entidad.Nombre);
        Assert.Equal("url", entidad.EscudoUrl);
        Assert.Equal("Campo Miralba", entidad.Sede);
        Assert.Equal("Sandra Vega", entidad.Entrenador);
    }

    [Fact]
    public void ToEntity_DeberiaInicializarColeccionJugadoresVacia()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "X", EscudoUrl = "", Sede = "S", Entrenador = "" };

        // ==================== ACT ====================
        var entidad = dto.ToEntity();

        // ==================== ASSERT ====================
        Assert.NotNull(entidad.Jugadores);
        Assert.Empty(entidad.Jugadores);
    }

    [Fact]
    public void ToEntity_NoDeberiaAsignarId()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "X", EscudoUrl = "", Sede = "S", Entrenador = "" };

        // ==================== ACT ====================
        var entidad = dto.ToEntity();

        // ==================== ASSERT ====================
        Assert.Equal(0, entidad.Id);
    }
}
