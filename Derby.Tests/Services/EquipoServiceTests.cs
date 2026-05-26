using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class EquipoServiceTests
{
    private readonly Mock<IEquipoRepository>      _mockEquipoRepo = new();
    private readonly Mock<ILigaRepository>        _mockLigaRepo   = new();
    private readonly Mock<ILogger<EquipoService>> _mockLogger     = new();
    private readonly EquipoService                _servicio;

    public EquipoServiceTests()
    {
        _servicio = new EquipoService(_mockEquipoRepo.Object, _mockLigaRepo.Object, _mockLogger.Object);
    }

    // =========================================================================
    // ObtenerTodos
    // =========================================================================

    [Fact]
    public async Task ObtenerTodosAsync_CuandoHayEquipos_DeberiaRetornarlosConSuLigaAsignada()
    {
        // ==================== ARRANGE ====================
        var equiposEnBD = new List<Equipo>
        {
            new() { Id = 1, Nombre = "FC Derby Norte", EscudoUrl = "", Sede = "Norte", Entrenador = "Coach A" },
            new() { Id = 2, Nombre = "UD Miralba",     EscudoUrl = "", Sede = "Sur",   Entrenador = "Coach B" },
        };
        var asignaciones = new List<LigaEquipo>
        {
            new() { EquipoId = 1, LigaId = 10, Liga = new Liga { Id = 10, Nombre = "Primera DAW", CompeticionId = 1 } }
        };

        _mockEquipoRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(equiposEnBD);
        _mockLigaRepo.Setup(r => r.ObtenerTodasAsignacionesAsync()).ReturnsAsync(asignaciones);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodosAsync();

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(2, resultado.Value.Count());

        // El equipo 1 tiene liga; el 2 no
        Assert.Equal("Primera DAW", resultado.Value.First(e => e.Id == 1).LigaNombre);
        Assert.Null(resultado.Value.First(e => e.Id == 2).LigaNombre);

        _mockEquipoRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        _mockLigaRepo.Verify(r => r.ObtenerTodasAsignacionesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObtenerTodosAsync_CuandoNoHayEquipos_DeberiaRetornarListaVacia()
    {
        // ==================== ARRANGE ====================
        _mockEquipoRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(new List<Equipo>());
        _mockLigaRepo.Setup(r => r.ObtenerTodasAsignacionesAsync()).ReturnsAsync(new List<LigaEquipo>());

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodosAsync();

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Empty(resultado.Value);
    }

    // =========================================================================
    // ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElEquipoExiste_DeberiaRetornarlo()
    {
        // ==================== ARRANGE ====================
        int idTest = 3;
        var equipoEnBD = new Equipo { Id = idTest, Nombre = "CD Las Torres", EscudoUrl = "", Sede = "Torres", Entrenador = "Coach C" };
        _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(equipoEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal("CD Las Torres", resultado.Value.Nombre);
        _mockEquipoRepo.Verify(r => r.ObtenerPorIdAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElEquipoNoExiste_DeberiaRetornarNotFoundError()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Equipo?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        Assert.Contains("99", resultado.Error.Message);
    }

    // =========================================================================
    // Crear
    // =========================================================================

    [Fact]
    public async Task CrearAsync_CuandoLosDatosSonValidos_DeberiaLlamarAlRepositorioYRetornarElEquipo()
    {
        // ==================== ARRANGE ====================
        var dto = new EquipoRequestDto { Nombre = "Racing Derby Club", EscudoUrl = "", Sede = "Estadio Derby", Entrenador = "Coach D" };
        var equipoCreado = new Equipo { Id = 7, Nombre = "Racing Derby Club", EscudoUrl = "", Sede = "Estadio Derby", Entrenador = "Coach D" };

        _mockEquipoRepo.Setup(r => r.CrearAsync(It.IsAny<Equipo>())).ReturnsAsync(equipoCreado);

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(7, resultado.Value.Id);
        Assert.Equal("Racing Derby Club", resultado.Value.Nombre);
        _mockEquipoRepo.Verify(r => r.CrearAsync(It.IsAny<Equipo>()), Times.Once);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoElEquipoExiste_DeberiaActualizarloYRetornarlo()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var equipoExistente = new Equipo { Id = idTest, Nombre = "Viejo", EscudoUrl = "", Sede = "Sede A", Entrenador = "Coach X" };
        var equipoActualizado = new Equipo { Id = idTest, Nombre = "Nuevo", EscudoUrl = "", Sede = "Sede B", Entrenador = "Coach Y" };
        var dto = new EquipoRequestDto { Nombre = "Nuevo", EscudoUrl = "", Sede = "Sede B", Entrenador = "Coach Y" };

        _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(equipoExistente);
        _mockEquipoRepo.Setup(r => r.ActualizarAsync(It.IsAny<Equipo>())).ReturnsAsync(equipoActualizado);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal("Nuevo", resultado.Value.Nombre);
        Assert.Equal("Sede B", resultado.Value.Sede);
        _mockEquipoRepo.Verify(r => r.ActualizarAsync(It.IsAny<Equipo>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElEquipoNoExiste_DeberiaRetornarNotFoundErrorYNOActualizar()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Equipo?)null);
        var dto = new EquipoRequestDto { Nombre = "X", EscudoUrl = "", Sede = "S", Entrenador = "" };

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idInexistente, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        _mockEquipoRepo.Verify(r => r.ActualizarAsync(It.IsAny<Equipo>()), Times.Never);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task EliminarAsync_CuandoElEquipoExiste_DeberiaRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        _mockEquipoRepo.Setup(r => r.EliminarAsync(idTest)).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.True(resultado.Value);
        _mockEquipoRepo.Verify(r => r.EliminarAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoElEquipoNoExiste_DeberiaRetornarNotFoundError()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockEquipoRepo.Setup(r => r.EliminarAsync(idInexistente)).ReturnsAsync(false);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        Assert.Contains("99", resultado.Error.Message);
    }
}
