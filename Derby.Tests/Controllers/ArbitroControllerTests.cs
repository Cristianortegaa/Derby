using CSharpFunctionalExtensions;
using Derby.Backend.Controllers;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Models;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Controllers;

/// <remarks>
/// ArbitroController usa DerbyContext directamente para ObtenerMisPartidos,
/// ObtenerPartidosPendientes, ObtenerHistorialPartidos y CrearActa.
/// Esos endpoints necesitarían EF InMemory; aquí solo se testean los que
/// delegan en servicios mockeables.
/// </remarks>
public class ArbitroControllerTests
{
    private readonly Mock<IArbitroService>             _mockArbitroService = new();
    private readonly Mock<IEventoPartidoService>       _mockEventoService  = new();
    private readonly Mock<ILogger<ArbitroController>>  _mockLogger         = new();
    private readonly ArbitroController                 _controller;

    public ArbitroControllerTests()
    {
        // DerbyContext no es necesario para los endpoints que se testean aquí.
        _controller = new ArbitroController(
            _mockArbitroService.Object,
            _mockEventoService.Object,
            null!,
            _mockLogger.Object);
    }

    // =========================================================================
    // ObtenerTodos
    // =========================================================================

    [Fact]
    public async Task ObtenerTodos_CuandoHayArbitros_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<ArbitroResponseDto>
        {
            new() { Id = 1, Nombre = "Carlos", Apellidos = "López", NumeroColegiado = "C-001" },
            new() { Id = 2, Nombre = "Marta",  Apellidos = "Ruiz",  NumeroColegiado = "C-002" },
        };
        _mockArbitroService.Setup(s => s.ObtenerTodosAsync())
                           .ReturnsAsync(Result.Success<IEnumerable<ArbitroResponseDto>, DerbyError>(lista));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerTodos();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(lista, result.Value);
    }

    [Fact]
    public async Task ObtenerTodos_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        _mockArbitroService.Setup(s => s.ObtenerTodosAsync())
                           .ReturnsAsync(Result.Failure<IEnumerable<ArbitroResponseDto>, DerbyError>(new BadRequestError("Error")));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerTodos();
        var result = actionResult.Result as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerPorId_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var arbitro = new ArbitroResponseDto { Id = 1, Nombre = "Carlos", Apellidos = "López" };
        _mockArbitroService.Setup(s => s.ObtenerPorIdAsync(1))
                           .ReturnsAsync(Result.Success<ArbitroResponseDto, DerbyError>(arbitro));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerPorId(1);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(arbitro, result.Value);
    }

    [Fact]
    public async Task ObtenerPorId_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockArbitroService.Setup(s => s.ObtenerPorIdAsync(99))
                           .ReturnsAsync(Result.Failure<ArbitroResponseDto, DerbyError>(new NotFoundError("Árbitro no encontrado")));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerPorId(99);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // Crear
    // =========================================================================

    [Fact]
    public async Task Crear_CuandoEsValido_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto     = new ArbitroRequestDto { Nombre = "Pedro", Apellidos = "García", NumeroColegiado = "C-010" };
        var arbitro = new ArbitroResponseDto { Id = 5, Nombre = dto.Nombre, Apellidos = dto.Apellidos };
        _mockArbitroService.Setup(s => s.CrearAsync(dto))
                           .ReturnsAsync(Result.Success<ArbitroResponseDto, DerbyError>(arbitro));

        // ==================== ACT ====================
        var actionResult = await _controller.Crear(dto);
        var result = actionResult.Result as CreatedAtActionResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
        Assert.Equal(arbitro, result.Value);
    }

    [Fact]
    public async Task Crear_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "", Apellidos = "", NumeroColegiado = "" };
        _mockArbitroService.Setup(s => s.CrearAsync(dto))
                           .ReturnsAsync(Result.Failure<ArbitroResponseDto, DerbyError>(new BadRequestError("Datos inválidos")));

        // ==================== ACT ====================
        var actionResult = await _controller.Crear(dto);
        var result = actionResult.Result as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task Actualizar_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto     = new ArbitroRequestDto { Nombre = "Pedro Editado", Apellidos = "García", NumeroColegiado = "C-010" };
        var arbitro = new ArbitroResponseDto { Id = 1, Nombre = dto.Nombre, Apellidos = dto.Apellidos };
        _mockArbitroService.Setup(s => s.ActualizarAsync(1, dto))
                           .ReturnsAsync(Result.Success<ArbitroResponseDto, DerbyError>(arbitro));

        // ==================== ACT ====================
        var actionResult = await _controller.Actualizar(1, dto);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task Actualizar_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "X", Apellidos = "Y", NumeroColegiado = "Z" };
        _mockArbitroService.Setup(s => s.ActualizarAsync(99, dto))
                           .ReturnsAsync(Result.Failure<ArbitroResponseDto, DerbyError>(new NotFoundError("Árbitro no encontrado")));

        // ==================== ACT ====================
        var actionResult = await _controller.Actualizar(99, dto);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task Eliminar_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        _mockArbitroService.Setup(s => s.EliminarAsync(1))
                           .ReturnsAsync(Result.Success<bool, DerbyError>(true));

        // ==================== ACT ====================
        var actionResult = await _controller.Eliminar(1);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task Eliminar_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockArbitroService.Setup(s => s.EliminarAsync(99))
                           .ReturnsAsync(Result.Failure<bool, DerbyError>(new NotFoundError("Árbitro no encontrado")));

        // ==================== ACT ====================
        var actionResult = await _controller.Eliminar(99);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerEventos
    // =========================================================================

    [Fact]
    public async Task ObtenerEventos_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        int partidoId = 1;
        var eventos = new List<EventoPartidoResponseDto>
        {
            new() { Id = 1, Minuto = 10, TipoEvento = "Gol" },
            new() { Id = 2, Minuto = 55, TipoEvento = "TarjetaAmarilla" },
        };
        _mockEventoService.Setup(s => s.ObtenerEventosAsync(partidoId)).ReturnsAsync(eventos);

        // ==================== ACT ====================
        var result = await _controller.ObtenerEventos(partidoId) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(eventos, result.Value);
    }

    // =========================================================================
    // AñadirEvento
    // =========================================================================

    [Fact]
    public async Task AñadirEvento_CuandoTipoValido_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        int partidoId = 1;
        var dto    = new EventoPartidoRequestDto { JugadorId = 3, Minuto = 25, TipoEvento = "Gol" };
        var evento = new EventoPartidoResponseDto { Id = 10, Minuto = 25, TipoEvento = "Gol" };
        _mockEventoService.Setup(s => s.AñadirEventoAsync(partidoId, dto)).ReturnsAsync(evento);

        // ==================== ACT ====================
        var result = await _controller.AñadirEvento(partidoId, dto) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(evento, result.Value);
    }

    [Fact]
    public async Task AñadirEvento_CuandoTipoInvalido_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        var dto = new EventoPartidoRequestDto { JugadorId = 1, Minuto = 10, TipoEvento = "TipoInexistente" };
        _mockEventoService.Setup(s => s.AñadirEventoAsync(1, dto)).ReturnsAsync((EventoPartidoResponseDto?)null);

        // ==================== ACT ====================
        var result = await _controller.AñadirEvento(1, dto) as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // EliminarEvento
    // =========================================================================

    [Fact]
    public async Task EliminarEvento_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        _mockEventoService.Setup(s => s.EliminarEventoAsync(1)).ReturnsAsync(true);

        // ==================== ACT ====================
        var result = await _controller.EliminarEvento(1, 1) as OkResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task EliminarEvento_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockEventoService.Setup(s => s.EliminarEventoAsync(99)).ReturnsAsync(false);

        // ==================== ACT ====================
        var result = await _controller.EliminarEvento(1, 99) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // CerrarActa
    // =========================================================================

    [Fact]
    public async Task CerrarActa_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        int partidoId = 1;
        var partido   = new Partido { Id = partidoId, GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado" };
        _mockEventoService.Setup(s => s.CerrarActaAsync(partidoId)).ReturnsAsync(partido);

        // ==================== ACT ====================
        var result = await _controller.CerrarActa(partidoId) as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(partido, result.Value);
    }

    [Fact]
    public async Task CerrarActa_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockEventoService.Setup(s => s.CerrarActaAsync(99)).ReturnsAsync((Partido?)null);

        // ==================== ACT ====================
        var result = await _controller.CerrarActa(99) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }
}


