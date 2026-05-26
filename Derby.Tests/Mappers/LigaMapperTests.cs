using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Xunit;

namespace Derby.Tests.Mappers;

public class LigaMapperTests
{
    [Fact]
    public void ToDto_DeberiaMapearTodosLosCampos()
    {
        // ==================== ARRANGE ====================
        var liga = new Liga
        {
            Id = 1, Nombre = "Primera DAW", CompeticionId = 5,
            Grupo = "Único", Jornadas = 38, JornadaActual = 10,
            Estado = "Activo",
            Partidos = new List<Partido>
            {
                new() { Estado = "Finalizado" },
                new() { Estado = "Finalizado" },
                new() { Estado = "Pendiente"  },
            }
        };

        // ==================== ACT ====================
        var dto = liga.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(1, dto.Id);
        Assert.Equal("Primera DAW", dto.Nombre);
        Assert.Equal(5, dto.CompeticionId);
        Assert.Equal(38, dto.Jornadas);
        Assert.Equal(10, dto.JornadaActual);
        Assert.Equal("Activo", dto.Estado);
        Assert.Equal(3, dto.TotalPartidos);
        Assert.Equal(2, dto.PartidosFinalizados);
    }

    [Fact]
    public void ToDto_SinPartidos_TotalYPartidosFinalizadosDeberianSerCero()
    {
        // ==================== ARRANGE ====================
        var liga = new Liga
        {
            Id = 2, Nombre = "L", CompeticionId = 1,
            Grupo = "Único", Jornadas = 10, JornadaActual = 0,
            Estado = "Activo", Partidos = new List<Partido>()
        };

        // ==================== ACT ====================
        var dto = liga.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(0, dto.TotalPartidos);
        Assert.Equal(0, dto.PartidosFinalizados);
    }
}
