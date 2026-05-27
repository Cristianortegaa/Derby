using CSharpFunctionalExtensions;
using Derby.Backend.Controllers;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Controllers;

public class AdminControllerTests
{
    private readonly Mock<ILogger<AdminController>>  _mockLogger             = new();
    private readonly Mock<ILigaService>              _mockLigaService        = new();
    private readonly Mock<IJugadorService>           _mockJugadorService     = new();
    private readonly Mock<IPartidoRepository>        _mockPartidoRepo        = new();
    private readonly Mock<ICompeticionService>       _mockCompeticionService = new();
    private readonly Mock<IArbitroService>           _mockArbitroService     = new();
    private readonly Mock<IPartidoService>           _mockPartidoService     = new();
    private readonly Mock<IEquipoService>            _mockEquipoService      = new();
    private readonly AdminController                 _controller;

    public AdminControllerTests()
    {
        _controller = new AdminController(
            _mockLogger.Object,
            _mockLigaService.Object,
            _mockJugadorService.Object,
            _mockPartidoRepo.Object,
            _mockCompeticionService.Object,
            _mockArbitroService.Object,
            _mockPartidoService.Object,
            _mockEquipoService.Object);
    }

    // =========================================================================
    // Competiciones
    // =========================================================================

    [Fact]
    public async Task ObtenerCompeticiones_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<CompeticionResponseDto>
        {
            new() { Id = 1, Nombre = "Copa RFEF", Temporada = "2024-25" },
        };
        _mockCompeticionService.Setup(s => s.ObtenerTodasAsync()).ReturnsAsync(lista);

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerCompeticiones();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task CrearCompeticion_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var competicion = new Competicion { Nombre = "Nueva Copa", Temporada = "2025-26" };
        var response    = new CompeticionResponseDto { Id = 5, Nombre = competicion.Nombre, Temporada = competicion.Temporada };
        _mockCompeticionService.Setup(s => s.CrearAsync(competicion)).ReturnsAsync(response);

        // ==================== ACT ====================
        var actionResult = await _controller.CrearCompeticion(competicion);
        var result = actionResult.Result as CreatedResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task ActualizarCompeticion_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var competicion = new Competicion { Nombre = "Nombre Editado", Temporada = "2025-26" };
        var response    = new CompeticionResponseDto { Id = 1, Nombre = competicion.Nombre };
        _mockCompeticionService.Setup(s => s.ActualizarAsync(1, competicion)).ReturnsAsync(response);

        // ==================== ACT ====================
        var result = await _controller.ActualizarCompeticion(1, competicion) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ActualizarCompeticion_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        var competicion = new Competicion { Nombre = "X", Temporada = "Y" };
        _mockCompeticionService.Setup(s => s.ActualizarAsync(99, competicion)).ReturnsAsync((CompeticionResponseDto?)null);

        // ==================== ACT ====================
        var result = await _controller.ActualizarCompeticion(99, competicion) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarCompeticion_CuandoExiste_DeberiaRetornar204()
    {
        // ==================== ARRANGE ====================
        _mockCompeticionService.Setup(s => s.EliminarAsync(1)).ReturnsAsync(true);

        // ==================== ACT ====================
        var result = await _controller.EliminarCompeticion(1) as NoContentResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(204, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarCompeticion_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockCompeticionService.Setup(s => s.EliminarAsync(99)).ReturnsAsync(false);

        // ==================== ACT ====================
        var result = await _controller.EliminarCompeticion(99) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // Ligas
    // =========================================================================

    [Fact]
    public async Task ObtenerLigas_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<LigaResponseDto> { new() { Id = 1, Nombre = "División 1" } };
        _mockLigaService.Setup(s => s.ObtenerTodasAsync()).ReturnsAsync(lista);

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerLigas();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ObtenerLiga_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var liga = new LigaResponseDto { Id = 1, Nombre = "División 1" };
        _mockLigaService.Setup(s => s.ObtenerPorIdAsync(1)).ReturnsAsync(liga);

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerLiga(1);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ObtenerLiga_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockLigaService.Setup(s => s.ObtenerPorIdAsync(99)).ReturnsAsync((LigaResponseDto?)null);

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerLiga(99);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    [Fact]
    public async Task CrearLiga_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto      = new LigaRequestDto { Nombre = "Nueva Liga", CompeticionId = 1 };
        var response = new LigaResponseDto { Id = 2, Nombre = dto.Nombre };
        _mockLigaService.Setup(s => s.CrearAsync(dto)).ReturnsAsync(response);

        // ==================== ACT ====================
        var actionResult = await _controller.CrearLiga(dto);
        var result = actionResult.Result as CreatedResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
    }

    [Fact]
    public async Task ActualizarLiga_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto      = new LigaRequestDto { Nombre = "Nombre Editado", CompeticionId = 1 };
        var response = new LigaResponseDto { Id = 1, Nombre = dto.Nombre };
        _mockLigaService.Setup(s => s.ActualizarAsync(1, dto)).ReturnsAsync(response);

        // ==================== ACT ====================
        var result = await _controller.ActualizarLiga(1, dto) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ActualizarLiga_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        var dto = new LigaRequestDto { Nombre = "X", CompeticionId = 1 };
        _mockLigaService.Setup(s => s.ActualizarAsync(99, dto)).ReturnsAsync((LigaResponseDto?)null);

        // ==================== ACT ====================
        var result = await _controller.ActualizarLiga(99, dto) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarLiga_CuandoExiste_DeberiaRetornar204()
    {
        // ==================== ARRANGE ====================
        _mockLigaService.Setup(s => s.EliminarAsync(1)).ReturnsAsync(true);

        // ==================== ACT ====================
        var result = await _controller.EliminarLiga(1) as NoContentResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(204, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarLiga_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockLigaService.Setup(s => s.EliminarAsync(99)).ReturnsAsync(false);

        // ==================== ACT ====================
        var result = await _controller.EliminarLiga(99) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // Equipos (vía AdminController)
    // =========================================================================

    [Fact]
    public async Task ObtenerEquipos_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<EquipoResponseDto> { new() { Id = 1, Nombre = "Real Derby" } };
        _mockEquipoService.Setup(s => s.ObtenerTodosAsync())
                          .ReturnsAsync(Result.Success<IEnumerable<EquipoResponseDto>, DerbyError>(lista));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerEquipos();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task CrearEquipo_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto    = new EquipoRequestDto { Nombre = "Nuevo Equipo", Sede = "Campo Sur", Entrenador = "Míster", EscudoUrl = "" };
        var equipo = new EquipoResponseDto { Id = 3, Nombre = dto.Nombre };
        _mockEquipoService.Setup(s => s.CrearAsync(dto))
                          .ReturnsAsync(Result.Success<EquipoResponseDto, DerbyError>(equipo));

        // ==================== ACT ====================
        var actionResult = await _controller.CrearEquipo(dto);
        var result = actionResult.Result as CreatedResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
    }

    // =========================================================================
    // Árbitros
    // =========================================================================

    [Fact]
    public async Task ObtenerArbitros_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<ArbitroResponseDto> { new() { Id = 1, Nombre = "Carlos", Apellidos = "López" } };
        _mockArbitroService.Setup(s => s.ObtenerTodosAsync())
                           .ReturnsAsync(Result.Success<IEnumerable<ArbitroResponseDto>, DerbyError>(lista));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerArbitros();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task CrearArbitro_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto      = new ArbitroRequestDto { Nombre = "Pedro", Apellidos = "García", NumeroColegiado = "C-001" };
        var response = new ArbitroResponseDto { Id = 2, Nombre = dto.Nombre, Apellidos = dto.Apellidos };
        _mockArbitroService.Setup(s => s.CrearAsync(dto))
                           .ReturnsAsync(Result.Success<ArbitroResponseDto, DerbyError>(response));

        // ==================== ACT ====================
        var actionResult = await _controller.CrearArbitro(dto);
        var result = actionResult.Result as CreatedResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
    }

    // =========================================================================
    // Partidos
    // =========================================================================

    [Fact]
    public async Task ObtenerPartidos_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<PartidoResponseDto> { new() { Id = 1, Estado = "Pendiente" } };
        _mockPartidoService.Setup(s => s.ObtenerTodosAsync()).ReturnsAsync(lista);

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerPartidos();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task CrearPartido_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto      = new PartidoRequestDto { LigaId = 1, Jornada = 1, EquipoLocalId = 1, EquipoVisitanteId = 2 };
        var response = new PartidoResponseDto { Id = 10, Estado = "Pendiente" };
        _mockPartidoService.Setup(s => s.CrearAsync(dto)).ReturnsAsync(response);

        // ==================== ACT ====================
        var actionResult = await _controller.CrearPartido(dto);
        var result = actionResult.Result as CreatedResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
    }

    [Fact]
    public async Task ActualizarPartido_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto      = new PartidoRequestDto { LigaId = 1, Jornada = 2, EquipoLocalId = 1, EquipoVisitanteId = 2 };
        var response = new PartidoResponseDto { Id = 1, Jornada = 2 };
        _mockPartidoService.Setup(s => s.ActualizarAsync(1, dto)).ReturnsAsync(response);

        // ==================== ACT ====================
        var result = await _controller.ActualizarPartido(1, dto) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ActualizarPartido_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        var dto = new PartidoRequestDto { LigaId = 1, Jornada = 1, EquipoLocalId = 1, EquipoVisitanteId = 2 };
        _mockPartidoService.Setup(s => s.ActualizarAsync(99, dto)).ReturnsAsync((PartidoResponseDto?)null);

        // ==================== ACT ====================
        var result = await _controller.ActualizarPartido(99, dto) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarPartido_CuandoExiste_DeberiaRetornar204()
    {
        // ==================== ARRANGE ====================
        _mockPartidoService.Setup(s => s.EliminarAsync(1)).ReturnsAsync(true);

        // ==================== ACT ====================
        var result = await _controller.EliminarPartido(1) as NoContentResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(204, result!.StatusCode);
    }

    // =========================================================================
    // Jugadores
    // =========================================================================

    [Fact]
    public async Task ObtenerJugadores_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<JugadorResponseDto> { new() { Id = 1, Nombre = "Jugador A", Dorsal = 9 } };
        _mockJugadorService.Setup(s => s.ObtenerPorEquipoAsync(1)).ReturnsAsync(lista);

        // ==================== ACT ====================
        var result = await _controller.ObtenerJugadores(1) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task AgregarJugador_CuandoEsValido_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto = new JugadorRequestDto { Nombre = "Nuevo Jugador", Dorsal = 7 };
        _mockJugadorService.Setup(s => s.AgregarAsync(1, dto)).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        var result = await _controller.AgregarJugador(1, dto) as OkResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task AgregarJugador_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        var dto = new JugadorRequestDto { Nombre = "Extra", Dorsal = 26 };
        _mockJugadorService.Setup(s => s.AgregarAsync(1, dto))
                           .ThrowsAsync(new Exception("El equipo ya tiene el máximo de 25 jugadores"));

        // ==================== ACT ====================
        var result = await _controller.AgregarJugador(1, dto) as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarJugador_CuandoEsValido_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        _mockJugadorService.Setup(s => s.EliminarAsync(1)).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        var result = await _controller.EliminarJugador(1) as OkResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarJugador_CuandoNoExiste_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        _mockJugadorService.Setup(s => s.EliminarAsync(99))
                           .ThrowsAsync(new Exception("Jugador no encontrado"));

        // ==================== ACT ====================
        var result = await _controller.EliminarJugador(99) as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // Actas
    // =========================================================================

    [Fact]
    public async Task ObtenerActas_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<Partido>
        {
            new() { Id = 1, Estado = "Finalizado", GolesLocal = 2, GolesVisitante = 1 },
        };
        _mockPartidoRepo.Setup(r => r.ObtenerFinalizadosAsync()).ReturnsAsync(lista);

        // ==================== ACT ====================
        var result = await _controller.ObtenerActas() as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task EditarActa_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var datos   = new Partido { GolesLocal = 3, GolesVisitante = 0 };
        var partido = new Partido { Id = 1, GolesLocal = 3, GolesVisitante = 0, Estado = "Finalizado" };
        _mockPartidoRepo.Setup(r => r.ActualizarGolesAsync(1, datos.GolesLocal, datos.GolesVisitante))
                        .ReturnsAsync(partido);

        // ==================== ACT ====================
        var result = await _controller.EditarActa(1, datos) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task EditarActa_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        var datos = new Partido { GolesLocal = 1, GolesVisitante = 1 };
        _mockPartidoRepo.Setup(r => r.ActualizarGolesAsync(99, datos.GolesLocal, datos.GolesVisitante))
                        .ReturnsAsync((Partido?)null);

        // ==================== ACT ====================
        var result = await _controller.EditarActa(99, datos) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }
}
