using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class PartidoServiceTests
{
    private readonly Mock<IPartidoRepository> _mockPartidoRepo = new();
    private readonly PartidoService           _servicio;

    public PartidoServiceTests()
    {
        _servicio = new PartidoService(_mockPartidoRepo.Object);
    }

    // =========================================================================
    // ObtenerTodos
    // =========================================================================

    [Fact]
    public async Task ObtenerTodosAsync_CuandoHayPartidos_DeberiaRetornarLaLista()
    {
        // ==================== ARRANGE ====================
        var partidosEnBD = new List<Partido>
        {
            new() { Id = 1, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2, Jornada = 1, Estado = "Pendiente",
                    EquipoLocal = new Equipo { Id = 1, Nombre = "A", EscudoUrl = "", Sede = "", Entrenador = "" },
                    EquipoVisitante = new Equipo { Id = 2, Nombre = "B", EscudoUrl = "", Sede = "", Entrenador = "" } },
            new() { Id = 2, LigaId = 1, EquipoLocalId = 3, EquipoVisitanteId = 4, Jornada = 2, Estado = "Finalizado",
                    EquipoLocal = new Equipo { Id = 3, Nombre = "C", EscudoUrl = "", Sede = "", Entrenador = "" },
                    EquipoVisitante = new Equipo { Id = 4, Nombre = "D", EscudoUrl = "", Sede = "", Entrenador = "" } },
        };
        _mockPartidoRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(partidosEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodosAsync();

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Pendiente", resultado.First().Estado);
        _mockPartidoRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObtenerTodosAsync_CuandoNoHayPartidos_DeberiaRetornarListaVacia()
    {
        // ==================== ARRANGE ====================
        _mockPartidoRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(new List<Partido>());

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodosAsync();

        // ==================== ASSERT ====================
        Assert.Empty(resultado);
        _mockPartidoRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
    }

    // =========================================================================
    // ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElPartidoExiste_DeberiaRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var partidoEnBD = new Partido
        {
            Id = idTest, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2, Jornada = 1, Estado = "Pendiente",
            EquipoLocal     = new Equipo { Id = 1, Nombre = "FC Derby Norte", EscudoUrl = "", Sede = "", Entrenador = "" },
            EquipoVisitante = new Equipo { Id = 2, Nombre = "UD Miralba",     EscudoUrl = "", Sede = "", Entrenador = "" }
        };
        _mockPartidoRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(partidoEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idTest);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal(idTest, resultado!.Id);
        Assert.Equal("Pendiente", resultado.Estado);
        _mockPartidoRepo.Verify(r => r.ObtenerPorIdAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElPartidoNoExiste_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockPartidoRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Partido?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
    }

    // =========================================================================
    // Crear
    // =========================================================================

    [Fact]
    public async Task CrearAsync_DeberiaLlamarAlRepositorioYRetornarElDto()
    {
        // ==================== ARRANGE ====================
        var dto = new PartidoRequestDto
        {
            Jornada = 1, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2,
            Estado = "Pendiente", FechaHora = DateTime.UtcNow.AddDays(7)
        };
        var partidoCreado = new Partido
        {
            Id = 10, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2, Jornada = 1, Estado = "Pendiente",
            EquipoLocal     = new Equipo { Id = 1, Nombre = "A", EscudoUrl = "", Sede = "", Entrenador = "" },
            EquipoVisitante = new Equipo { Id = 2, Nombre = "B", EscudoUrl = "", Sede = "", Entrenador = "" }
        };
        _mockPartidoRepo.Setup(r => r.CrearAsync(It.IsAny<Partido>())).ReturnsAsync(partidoCreado);

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.Equal(10, resultado.Id);
        Assert.Equal("Pendiente", resultado.Estado);
        _mockPartidoRepo.Verify(r => r.CrearAsync(It.IsAny<Partido>()), Times.Once);
    }

    [Fact]
    public async Task CrearAsync_DeberiaMapearCorrectamenteTodosLosCamposDelDto()
    {
        // ==================== ARRANGE ====================
        var fechaTest = new DateTime(2026, 9, 5, 18, 0, 0, DateTimeKind.Utc);
        var dto = new PartidoRequestDto
        {
            Jornada = 3, LigaId = 2, EquipoLocalId = 5, EquipoVisitanteId = 6,
            GolesLocal = null, GolesVisitante = null, Estado = "Pendiente",
            FechaHora = fechaTest, ArbitroId = 4
        };

        // El mock sólo acepta el partido si tiene los campos exactos del DTO
        _mockPartidoRepo
            .Setup(r => r.CrearAsync(It.Is<Partido>(p =>
                p.Jornada           == 3 &&
                p.LigaId            == 2 &&
                p.EquipoLocalId     == 5 &&
                p.EquipoVisitanteId == 6 &&
                p.ArbitroId         == 4
            )))
            .ReturnsAsync(new Partido
            {
                Id = 1, LigaId = 2, EquipoLocalId = 5, EquipoVisitanteId = 6, Jornada = 3, Estado = "Pendiente",
                EquipoLocal     = new Equipo { Id = 5, Nombre = "E", EscudoUrl = "", Sede = "", Entrenador = "" },
                EquipoVisitante = new Equipo { Id = 6, Nombre = "F", EscudoUrl = "", Sede = "", Entrenador = "" }
            });

        // ==================== ACT ====================
        await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        // Si el setup se cumplió, el repo fue llamado exactamente una vez con los campos correctos
        _mockPartidoRepo.Verify(r => r.CrearAsync(It.IsAny<Partido>()), Times.Once);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoElPartidoExiste_DeberiaRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var dto = new PartidoRequestDto
        {
            Jornada = 1, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2,
            GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado", FechaHora = DateTime.UtcNow
        };
        var partidoActualizado = new Partido
        {
            Id = idTest, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2,
            GolesLocal = 2, GolesVisitante = 1, Jornada = 1, Estado = "Finalizado",
            EquipoLocal     = new Equipo { Id = 1, Nombre = "A", EscudoUrl = "", Sede = "", Entrenador = "" },
            EquipoVisitante = new Equipo { Id = 2, Nombre = "B", EscudoUrl = "", Sede = "", Entrenador = "" }
        };
        _mockPartidoRepo.Setup(r => r.ActualizarAsync(idTest, It.IsAny<Partido>())).ReturnsAsync(partidoActualizado);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal("Finalizado", resultado!.Estado);
        Assert.Equal(2, resultado.GolesLocal);
        _mockPartidoRepo.Verify(r => r.ActualizarAsync(idTest, It.IsAny<Partido>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElPartidoNoExiste_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        var dto = new PartidoRequestDto
        {
            Jornada = 1, LigaId = 1, EquipoLocalId = 1, EquipoVisitanteId = 2,
            Estado = "Pendiente", FechaHora = DateTime.UtcNow
        };
        _mockPartidoRepo.Setup(r => r.ActualizarAsync(idInexistente, It.IsAny<Partido>())).ReturnsAsync((Partido?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idInexistente, dto);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task EliminarAsync_CuandoElPartidoExiste_DeberiaRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        _mockPartidoRepo.Setup(r => r.EliminarAsync(idTest)).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado);
        _mockPartidoRepo.Verify(r => r.EliminarAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoElPartidoNoExiste_DeberiaRetornarFalse()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockPartidoRepo.Setup(r => r.EliminarAsync(idInexistente)).ReturnsAsync(false);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.False(resultado);
        _mockPartidoRepo.Verify(r => r.EliminarAsync(idInexistente), Times.Once);
    }
}
