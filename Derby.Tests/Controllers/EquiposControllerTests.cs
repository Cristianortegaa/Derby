using CSharpFunctionalExtensions;
using Derby.Backend.Controllers;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Controllers;

public class EquiposControllerTests
{
    private readonly Mock<IEquipoService>             _mockService = new();
    private readonly Mock<ILogger<EquiposController>> _mockLogger  = new();
    private readonly EquiposController                _controller;

    public EquiposControllerTests()
    {
        _controller = new EquiposController(_mockService.Object, _mockLogger.Object);
    }

    // =========================================================================
    // GetEquipos
    // =========================================================================

    [Fact]
    public async Task GetEquipos_CuandoHayEquipos_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var lista = new List<EquipoResponseDto>
        {
            new() { Id = 1, Nombre = "Real Derby", Sede = "Estadio Derby" },
            new() { Id = 2, Nombre = "Derby FC",   Sede = "Campo Norte"  },
        };
        _mockService.Setup(s => s.ObtenerTodosAsync())
                    .ReturnsAsync(Result.Success<IEnumerable<EquipoResponseDto>, DerbyError>(lista));

        // ==================== ACT ====================
        var actionResult = await _controller.GetEquipos();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task GetEquipos_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.ObtenerTodosAsync())
                    .ReturnsAsync(Result.Failure<IEnumerable<EquipoResponseDto>, DerbyError>(new BadRequestError("Error interno")));

        // ==================== ACT ====================
        var actionResult = await _controller.GetEquipos();
        var result = actionResult.Result as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // GetEquipoById
    // =========================================================================

    [Fact]
    public async Task GetEquipoById_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var equipo = new EquipoResponseDto { Id = 1, Nombre = "Real Derby", Sede = "Estadio Derby" };
        _mockService.Setup(s => s.ObtenerPorIdAsync(1))
                    .ReturnsAsync(Result.Success<EquipoResponseDto, DerbyError>(equipo));

        // ==================== ACT ====================
        var actionResult = await _controller.GetEquipoById(1);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(equipo, result.Value);
    }

    [Fact]
    public async Task GetEquipoById_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.ObtenerPorIdAsync(99))
                    .ReturnsAsync(Result.Failure<EquipoResponseDto, DerbyError>(new NotFoundError("Equipo no encontrado")));

        // ==================== ACT ====================
        var actionResult = await _controller.GetEquipoById(99);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // CreateEquipo
    // =========================================================================

    [Fact]
    public async Task CreateEquipo_CuandoEsValido_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto    = new EquipoRequestDto { Nombre = "Nuevo Equipo", Sede = "Campo Sur", Entrenador = "Míster", EscudoUrl = "" };
        var equipo = new EquipoResponseDto { Id = 3, Nombre = dto.Nombre, Sede = dto.Sede };
        _mockService.Setup(s => s.CrearAsync(dto))
                    .ReturnsAsync(Result.Success<EquipoResponseDto, DerbyError>(equipo));

        // ==================== ACT ====================
        var actionResult = await _controller.CreateEquipo(dto);
        var result = actionResult.Result as CreatedAtActionResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
        Assert.Equal(equipo, result.Value);
    }

    [Fact]
    public async Task CreateEquipo_CuandoYaEstaInscrito_DeberiaRetornar409()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "Equipo Duplicado", Sede = "Campo Sur", Entrenador = "Míster", EscudoUrl = "" };
        _mockService.Setup(s => s.CrearAsync(dto))
                    .ReturnsAsync(Result.Failure<EquipoResponseDto, DerbyError>(new EquipoYaInscritoError("El equipo ya está inscrito")));

        // ==================== ACT ====================
        var actionResult = await _controller.CreateEquipo(dto);
        var result = actionResult.Result as ConflictObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(409, result!.StatusCode);
    }

    [Fact]
    public async Task CreateEquipo_CuandoOtroError_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "", Sede = "", Entrenador = "", EscudoUrl = "" };
        _mockService.Setup(s => s.CrearAsync(dto))
                    .ReturnsAsync(Result.Failure<EquipoResponseDto, DerbyError>(new BadRequestError("Datos inválidos")));

        // ==================== ACT ====================
        var actionResult = await _controller.CreateEquipo(dto);
        var result = actionResult.Result as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // UpdateEquipo
    // =========================================================================

    [Fact]
    public async Task UpdateEquipo_CuandoEsValido_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto    = new EquipoRequestDto { Nombre = "Nombre Editado", Sede = "Campo Este", Entrenador = "Nuevo Míster", EscudoUrl = "" };
        var equipo = new EquipoResponseDto { Id = 1, Nombre = dto.Nombre, Sede = dto.Sede };
        _mockService.Setup(s => s.ActualizarAsync(1, dto))
                    .ReturnsAsync(Result.Success<EquipoResponseDto, DerbyError>(equipo));

        // ==================== ACT ====================
        var actionResult = await _controller.UpdateEquipo(1, dto);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(equipo, result.Value);
    }

    [Fact]
    public async Task UpdateEquipo_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "X", Sede = "Y", Entrenador = "Z", EscudoUrl = "" };
        _mockService.Setup(s => s.ActualizarAsync(99, dto))
                    .ReturnsAsync(Result.Failure<EquipoResponseDto, DerbyError>(new NotFoundError("Equipo no encontrado")));

        // ==================== ACT ====================
        var actionResult = await _controller.UpdateEquipo(99, dto);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // DeleteEquipo
    // =========================================================================

    [Fact]
    public async Task DeleteEquipo_CuandoEsValido_DeberiaRetornar204()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.EliminarAsync(1))
                    .ReturnsAsync(Result.Success<bool, DerbyError>(true));

        // ==================== ACT ====================
        var result = await _controller.DeleteEquipo(1) as NoContentResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(204, result!.StatusCode);
    }

    [Fact]
    public async Task DeleteEquipo_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.EliminarAsync(99))
                    .ReturnsAsync(Result.Failure<bool, DerbyError>(new NotFoundError("Equipo no encontrado")));

        // ==================== ACT ====================
        var result = await _controller.DeleteEquipo(99) as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }
}


