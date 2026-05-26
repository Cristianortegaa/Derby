using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Xunit;

namespace Derby.Tests.Mappers;

public class PartidoMapperTests
{
    private static Partido PartidoConEquipos(int golesLocal = 2, int golesVisitante = 1, string estado = "Finalizado") =>
        new()
        {
            Id              = 1,
            Jornada         = 3,
            LigaId          = 1,
            FechaHora       = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
            GolesLocal      = golesLocal,
            GolesVisitante  = golesVisitante,
            Estado          = estado,
            EquipoLocal     = new Equipo { Id = 1, Nombre = "FC Derby Norte", EscudoUrl = "", Sede = "", Entrenador = "" },
            EquipoVisitante = new Equipo { Id = 2, Nombre = "UD Miralba",     EscudoUrl = "", Sede = "", Entrenador = "" }
        };

    [Fact]
    public void ToDto_DeberiaMapearGoles()
    {
        // ==================== ARRANGE ====================
        var partido = PartidoConEquipos(golesLocal: 3, golesVisitante: 0);

        // ==================== ACT ====================
        var dto = partido.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal(3, dto.GolesLocal);
        Assert.Equal(0, dto.GolesVisitante);
    }

    [Fact]
    public void ToDto_DeberiaMapearEstado()
    {
        // ==================== ARRANGE ====================
        var partido = PartidoConEquipos(estado: "Pendiente");

        // ==================== ACT ====================
        var dto = partido.ToDto();

        // ==================== ASSERT ====================
        Assert.Equal("Pendiente", dto.Estado);
    }

    [Fact]
    public void ToDto_DeberiaMapearEquiposComoSubDtos()
    {
        // ==================== ARRANGE ====================
        var partido = PartidoConEquipos();

        // ==================== ACT ====================
        var dto = partido.ToDto();

        // ==================== ASSERT ====================
        Assert.NotNull(dto.EquipoLocal);
        Assert.Equal("FC Derby Norte", dto.EquipoLocal!.Nombre);
        Assert.NotNull(dto.EquipoVisitante);
        Assert.Equal("UD Miralba", dto.EquipoVisitante!.Nombre);
    }

    [Fact]
    public void ToDto_SinEquiposAsignados_EquiposDeberianSerNull()
    {
        // ==================== ARRANGE ====================
        var partido = new Partido
        {
            Id = 2, Jornada = 1, LigaId = 1, Estado = "Pendiente",
            FechaHora = DateTime.UtcNow, GolesLocal = null, GolesVisitante = null
        };

        // ==================== ACT ====================
        var dto = partido.ToDto();

        // ==================== ASSERT ====================
        Assert.Null(dto.EquipoLocal);
        Assert.Null(dto.EquipoVisitante);
    }

    [Fact]
    public void ToResultadoDto_DeberiaMapearNombresYGoles()
    {
        // ==================== ARRANGE ====================
        var partido = PartidoConEquipos(golesLocal: 2, golesVisitante: 1);

        // ==================== ACT ====================
        var dto = partido.ToResultadoDto();

        // ==================== ASSERT ====================
        Assert.Equal("FC Derby Norte", dto.EquipoLocal);
        Assert.Equal("UD Miralba", dto.EquipoVisitante);
        Assert.Equal(2, dto.GolesLocal);
        Assert.Equal(1, dto.GolesVisitante);
    }

    [Fact]
    public void ToResultadoDto_SinEquiposAsignados_DeberiaUsarDesconocido()
    {
        // ==================== ARRANGE ====================
        var partido = new Partido
        {
            Id = 3, Jornada = 1, LigaId = 1, Estado = "Finalizado",
            FechaHora = DateTime.UtcNow, GolesLocal = 0, GolesVisitante = 0
        };

        // ==================== ACT ====================
        var dto = partido.ToResultadoDto();

        // ==================== ASSERT ====================
        Assert.Equal("Desconocido", dto.EquipoLocal);
        Assert.Equal("Desconocido", dto.EquipoVisitante);
    }
}
