using Derby.Backend.Controllers;
using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Controllers;

public class CompeticionesControllerTests
{
    private readonly Mock<ICompeticionService>               _mockCompeticionService = new();
    private readonly Mock<ILigaService>                      _mockLigaService        = new();
    private readonly Mock<ILogger<CompeticionesController>>  _mockLogger             = new();
    private readonly CompeticionesController                 _controller;

    public CompeticionesControllerTests()
    {
        _controller = new CompeticionesController(
            _mockCompeticionService.Object,
            _mockLigaService.Object,
            _mockLogger.Object);
    }

    // =========================================================================
    // ObtenerJornadas (por competición)
    // =========================================================================

    [Fact]
    public async Task ObtenerJornadas_CuandoExistenDatos_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var jornadas = new List<JornadaResponseDto>
        {
            new() { Numero = 1, Partidos = new List<PartidoResponseDto>() },
            new() { Numero = 2, Partidos = new List<PartidoResponseDto>() },
        };
        _mockCompeticionService.Setup(s => s.ObtenerJornadasAsync(1)).ReturnsAsync(jornadas);

        // ==================== ACT ====================
        var result = await _controller.ObtenerJornadas(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(jornadas, result.Value);
    }

    [Fact]
    public async Task ObtenerJornadas_CuandoExcepcion_DeberiaRetornar500()
    {
        // ==================== ARRANGE ====================
        _mockCompeticionService.Setup(s => s.ObtenerJornadasAsync(99))
                               .ThrowsAsync(new Exception("Error de BD"));

        // ==================== ACT ====================
        var result = await _controller.ObtenerJornadas(99) as ObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(500, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerResultados (por competición)
    // =========================================================================

    [Fact]
    public async Task ObtenerResultados_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var resultados = new List<ResultadoPartidoResponseDto>
        {
            new() { Id = 1, GolesLocal = 2, GolesVisitante = 1 },
        };
        _mockCompeticionService.Setup(s => s.ObtenerResultadosAsync(1)).ReturnsAsync(resultados);

        // ==================== ACT ====================
        var result = await _controller.ObtenerResultados(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerClasificacion (por competición)
    // =========================================================================

    [Fact]
    public async Task ObtenerClasificacion_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var clasificacion = new List<EquipoClasificacionResponseDto>
        {
            new() { Id = 1, Nombre = "Real Derby", Puntos = 10 },
        };
        _mockCompeticionService.Setup(s => s.ObtenerClasificacionAsync(1)).ReturnsAsync(clasificacion);

        // ==================== ACT ====================
        var result = await _controller.ObtenerClasificacion(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerGoleadores (por competición)
    // =========================================================================

    [Fact]
    public async Task ObtenerGoleadores_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var goleadores = new List<GoleadorResponseDto>
        {
            new() { Id = 1, Nombre = "Jugador A", Goles = 5 },
        };
        _mockCompeticionService.Setup(s => s.ObtenerGoleadoresAsync(1)).ReturnsAsync(goleadores);

        // ==================== ACT ====================
        var result = await _controller.ObtenerGoleadores(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerJornadasPorLiga
    // =========================================================================

    [Fact]
    public async Task ObtenerJornadasPorLiga_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var jornadas = new List<JornadaResponseDto> { new() { Numero = 1 } };
        _mockLigaService.Setup(s => s.ObtenerJornadasAsync(1)).ReturnsAsync(jornadas);

        // ==================== ACT ====================
        var result = await _controller.ObtenerJornadasPorLiga(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ObtenerJornadasPorLiga_CuandoExcepcion_DeberiaRetornar500()
    {
        // ==================== ARRANGE ====================
        _mockLigaService.Setup(s => s.ObtenerJornadasAsync(99))
                        .ThrowsAsync(new Exception("Error de BD"));

        // ==================== ACT ====================
        var result = await _controller.ObtenerJornadasPorLiga(99) as ObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(500, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerResultadosPorLiga
    // =========================================================================

    [Fact]
    public async Task ObtenerResultadosPorLiga_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var resultados = new List<ResultadoPartidoResponseDto>();
        _mockLigaService.Setup(s => s.ObtenerResultadosAsync(1)).ReturnsAsync(resultados);

        // ==================== ACT ====================
        var result = await _controller.ObtenerResultadosPorLiga(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerClasificacionPorLiga
    // =========================================================================

    [Fact]
    public async Task ObtenerClasificacionPorLiga_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var clasificacion = new List<EquipoClasificacionResponseDto>();
        _mockLigaService.Setup(s => s.ObtenerClasificacionAsync(1)).ReturnsAsync(clasificacion);

        // ==================== ACT ====================
        var result = await _controller.ObtenerClasificacionPorLiga(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerGoleadoresPorLiga
    // =========================================================================

    [Fact]
    public async Task ObtenerGoleadoresPorLiga_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var goleadores = new List<GoleadorResponseDto>();
        _mockLigaService.Setup(s => s.ObtenerGoleadoresAsync(1)).ReturnsAsync(goleadores);

        // ==================== ACT ====================
        var result = await _controller.ObtenerGoleadoresPorLiga(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    // =========================================================================
    // BuscarCompeticiones
    // =========================================================================

    [Fact]
    public async Task BuscarCompeticiones_ConFiltros_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var competiciones = new List<Competicion>
        {
            new() { Id = 1, Nombre = "Copa RFEF", Temporada = "2024-25" },
        };
        _mockCompeticionService
            .Setup(s => s.BuscarCompeticionesAsync("2024-25", "Liga", null, null))
            .ReturnsAsync(competiciones);

        // ==================== ACT ====================
        var result = await _controller.BuscarCompeticiones("2024-25", "Liga", null, null) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(competiciones, result.Value);
    }

    [Fact]
    public async Task BuscarCompeticiones_SinFiltros_DeberiaRetornarTodas()
    {
        // ==================== ARRANGE ====================
        var competiciones = new List<Competicion>
        {
            new() { Id = 1, Nombre = "Copa RFEF" },
            new() { Id = 2, Nombre = "Liga Juvenil" },
            new() { Id = 3, Nombre = "Torneo Verano" },
        };
        _mockCompeticionService
            .Setup(s => s.BuscarCompeticionesAsync(null, null, null, null))
            .ReturnsAsync(competiciones);

        // ==================== ACT ====================
        var result = await _controller.BuscarCompeticiones(null, null, null, null) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        var lista = Assert.IsType<List<Competicion>>(result.Value);
        Assert.Equal(3, lista.Count);
    }
}


