using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class ArbitroServiceTests
{
    // ── Mocks de dependencias (equivalente a @Mock en Mockito) ────────────────
    private readonly Mock<IArbitroRepository>      _mockRepositorio = new();
    private readonly Mock<ILogger<ArbitroService>> _mockLogger      = new();
    private readonly ArbitroService                _servicio;

    public ArbitroServiceTests()
    {
        // Inyectamos los mocks con .Object, igual que Mockito usa @InjectMocks
        _servicio = new ArbitroService(_mockRepositorio.Object, _mockLogger.Object);
    }

    // =========================================================================
    // ObtenerTodos
    // =========================================================================

    [Fact]
    public async Task ObtenerTodosAsync_CuandoExistenArbitros_DeberiaRetornarLaLista()
    {
        // ==================== ARRANGE ====================
        var arbitrosEnBD = new List<Arbitro>
        {
            new() { Id = 1, Nombre = "Carlos", Apellidos = "García",  NumeroColegiado = "C001" },
            new() { Id = 2, Nombre = "María",  Apellidos = "López",   NumeroColegiado = "C002" },
        };
        _mockRepositorio
            .Setup(r => r.ObtenerTodosAsync())
            .ReturnsAsync(arbitrosEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodosAsync();

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(2, resultado.Value.Count());
        Assert.Equal("Carlos", resultado.Value.First().Nombre);

        // Verificamos que el repositorio fue consultado exactamente una vez
        _mockRepositorio.Verify(r => r.ObtenerTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObtenerTodosAsync_CuandoNoHayArbitros_DeberiaRetornarListaVacia()
    {
        // ==================== ARRANGE ====================
        _mockRepositorio
            .Setup(r => r.ObtenerTodosAsync())
            .ReturnsAsync(new List<Arbitro>());

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
    public async Task ObtenerPorIdAsync_CuandoElIdExiste_DeberiaRetornarElArbitro()
    {
        // ==================== ARRANGE ====================
        int idTest = 5;
        var arbitroEnBD = new Arbitro { Id = idTest, Nombre = "Juan", Apellidos = "Pérez", NumeroColegiado = "C005" };
        _mockRepositorio
            .Setup(r => r.ObtenerPorIdAsync(idTest))
            .ReturnsAsync(arbitroEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(idTest, resultado.Value.Id);
        Assert.Equal("Juan", resultado.Value.Nombre);
        _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElIdNoExiste_DeberiaRetornarNotFoundError()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockRepositorio
            .Setup(r => r.ObtenerPorIdAsync(idInexistente))
            .ReturnsAsync((Arbitro?)null);

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
    public async Task CrearAsync_CuandoLosDatosSonValidos_DeberiaLlamarAlRepositorioYRetornarElArbitro()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "Luis", Apellidos = "Martínez", NumeroColegiado = "C010" };
        var arbitroCreado = new Arbitro { Id = 10, Nombre = "Luis", Apellidos = "Martínez", NumeroColegiado = "C010" };

        _mockRepositorio
            .Setup(r => r.CrearAsync(It.IsAny<Arbitro>()))
            .ReturnsAsync(arbitroCreado);

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(10, resultado.Value.Id);
        Assert.Equal("Luis", resultado.Value.Nombre);

        // El repositorio debe haber sido llamado exactamente una vez
        _mockRepositorio.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Once);
    }

    [Fact]
    public async Task CrearAsync_CuandoElNombreEstaVacio_DeberiaRetornarBadRequestErrorYNOLlamarAlRepositorio()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "", Apellidos = "Martínez", NumeroColegiado = "C010" };

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);

        // NUNCA debe llegar al repositorio si la validación falla
        _mockRepositorio.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_CuandoElNombreSoloTieneEspacios_DeberiaRetornarBadRequestError()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "   ", Apellidos = "Martínez", NumeroColegiado = "C010" };

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);
        _mockRepositorio.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_CuandoLosApellidosEstanVacios_DeberiaRetornarBadRequestErrorYNOLlamarAlRepositorio()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "Luis", Apellidos = "", NumeroColegiado = "C010" };

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);
        _mockRepositorio.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_CuandoLosApellidosSoloTienenEspacios_DeberiaRetornarBadRequestError()
    {
        // ==================== ARRANGE ====================
        var dto = new ArbitroRequestDto { Nombre = "Luis", Apellidos = "   ", NumeroColegiado = "C010" };

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);
        _mockRepositorio.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoElIdExisteYLosDatosSonValidos_DeberiaActualizarYRetornar()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var arbitroExistente = new Arbitro { Id = idTest, Nombre = "Viejo", Apellidos = "Nombre", NumeroColegiado = "OLD" };
        var arbitroActualizado = new Arbitro { Id = idTest, Nombre = "Nuevo", Apellidos = "Apellido", NumeroColegiado = "NEW" };
        var dto = new ArbitroRequestDto { Nombre = "Nuevo", Apellidos = "Apellido", NumeroColegiado = "NEW" };

        _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(arbitroExistente);
        _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Arbitro>())).ReturnsAsync(arbitroActualizado);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal("Nuevo", resultado.Value.Nombre);
        Assert.Equal("NEW", resultado.Value.NumeroColegiado);
        _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Arbitro>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElIdNoExiste_DeberiaRetornarNotFoundErrorYNOActualizar()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Arbitro?)null);
        var dto = new ArbitroRequestDto { Nombre = "X", Apellidos = "Y", NumeroColegiado = "Z" };

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idInexistente, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElNombreEstaVacioEnLaActualizacion_DeberiaRetornarBadRequestError()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var arbitroExistente = new Arbitro { Id = idTest, Nombre = "A", Apellidos = "B", NumeroColegiado = "" };
        _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(arbitroExistente);
        var dto = new ArbitroRequestDto { Nombre = "", Apellidos = "B", NumeroColegiado = "" };

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);
        _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task EliminarAsync_CuandoElIdExiste_DeberiaRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        _mockRepositorio.Setup(r => r.EliminarAsync(idTest)).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.True(resultado.Value);
        _mockRepositorio.Verify(r => r.EliminarAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoElIdNoExiste_DeberiaRetornarNotFoundError()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockRepositorio.Setup(r => r.EliminarAsync(idInexistente)).ReturnsAsync(false);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        Assert.Contains("99", resultado.Error.Message);
        _mockRepositorio.Verify(r => r.EliminarAsync(idInexistente), Times.Once);
    }
}
