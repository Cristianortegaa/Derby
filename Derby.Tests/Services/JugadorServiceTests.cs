using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class JugadorServiceTests
{
    private readonly Mock<IJugadorRepository> _mockJugadorRepo = new();
    private readonly JugadorService           _servicio;

    public JugadorServiceTests()
    {
        _servicio = new JugadorService(_mockJugadorRepo.Object);
    }

    // =========================================================================
    // ObtenerPorEquipo
    // =========================================================================

    [Fact]
    public async Task ObtenerPorEquipoAsync_CuandoHayJugadores_DeberiaRetornarLaLista()
    {
        // ==================== ARRANGE ====================
        int equipoId = 1;
        var jugadoresEnBD = new List<Jugador>
        {
            new() { Id = 1, Nombre = "Jugador A", Dorsal = 9,  EquipoId = equipoId },
            new() { Id = 2, Nombre = "Jugador B", Dorsal = 10, EquipoId = equipoId },
        };
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(jugadoresEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorEquipoAsync(equipoId);

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Jugador A", resultado[0].Nombre);
        _mockJugadorRepo.Verify(r => r.ObtenerPorEquipoAsync(equipoId), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorEquipoAsync_CuandoNoHayJugadores_DeberiaRetornarListaVacia()
    {
        // ==================== ARRANGE ====================
        int equipoId = 99;
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(new List<Jugador>());

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorEquipoAsync(equipoId);

        // ==================== ASSERT ====================
        Assert.Empty(resultado);
    }

    // =========================================================================
    // Agregar
    // =========================================================================

    [Fact]
    public async Task AgregarAsync_CuandoLosDatosSonValidos_DeberiaLlamarAlRepositorio()
    {
        // ==================== ARRANGE ====================
        int equipoId = 1;
        var dto = new JugadorRequestDto { Nombre = "Nuevo Jugador", Dorsal = 7 };
        var jugadoresExistentes = new List<Jugador>
        {
            new() { Id = 1, Nombre = "Jugador A", Dorsal = 9, EquipoId = equipoId }
        };
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(jugadoresExistentes);
        _mockJugadorRepo.Setup(r => r.AgregarAsync(It.IsAny<Jugador>())).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        await _servicio.AgregarAsync(equipoId, dto);

        // ==================== ASSERT ====================
        _mockJugadorRepo.Verify(r => r.AgregarAsync(It.IsAny<Jugador>()), Times.Once);
    }

    [Fact]
    public async Task AgregarAsync_CuandoElEquipoTiene25Jugadores_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int equipoId = 1;
        var dto = new JugadorRequestDto { Nombre = "Extra", Dorsal = 26 };
        var jugadoresFull = Enumerable.Range(1, 25)
            .Select(i => new Jugador { Id = i, Nombre = $"Jugador {i}", Dorsal = i, EquipoId = equipoId })
            .ToList();
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(jugadoresFull);

        // ==================== ACT & ASSERT ====================
        var ex = await Assert.ThrowsAsync<Exception>(() => _servicio.AgregarAsync(equipoId, dto));
        Assert.Contains("máximo de 25 jugadores", ex.Message);
        _mockJugadorRepo.Verify(r => r.AgregarAsync(It.IsAny<Jugador>()), Times.Never);
    }

    [Fact]
    public async Task AgregarAsync_CuandoElDorsalYaExiste_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int equipoId = 1;
        var dto = new JugadorRequestDto { Nombre = "Jugador Nuevo", Dorsal = 9 };
        var jugadoresExistentes = new List<Jugador>
        {
            new() { Id = 1, Nombre = "Jugador A", Dorsal = 9, EquipoId = equipoId }
        };
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(jugadoresExistentes);

        // ==================== ACT & ASSERT ====================
        var ex = await Assert.ThrowsAsync<Exception>(() => _servicio.AgregarAsync(equipoId, dto));
        Assert.Contains("dorsal", ex.Message);
        _mockJugadorRepo.Verify(r => r.AgregarAsync(It.IsAny<Jugador>()), Times.Never);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoElJugadorExisteYElDorsalEsLibre_DeberiaActualizar()
    {
        // ==================== ARRANGE ====================
        int jugadorId = 1;
        int equipoId  = 1;
        var dto = new JugadorRequestDto { Nombre = "Nombre Actualizado", Dorsal = 11 };

        var jugadorEnBD = new Jugador { Id = jugadorId, Nombre = "Jugador A", Dorsal = 9, EquipoId = equipoId };
        var compañeros = new List<Jugador>
        {
            jugadorEnBD,
            new() { Id = 2, Nombre = "Jugador B", Dorsal = 5, EquipoId = equipoId }
        };

        _mockJugadorRepo.Setup(r => r.ObtenerPorIdAsync(jugadorId)).ReturnsAsync(jugadorEnBD);
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(compañeros);
        _mockJugadorRepo.Setup(r => r.ActualizarAsync(It.IsAny<Jugador>())).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        await _servicio.ActualizarAsync(jugadorId, dto);

        // ==================== ASSERT ====================
        _mockJugadorRepo.Verify(r => r.ActualizarAsync(It.Is<Jugador>(j => j.Nombre == "Nombre Actualizado" && j.Dorsal == 11)), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElJugadorNoExiste_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int jugadorId = 99;
        _mockJugadorRepo.Setup(r => r.ObtenerPorIdAsync(jugadorId)).ReturnsAsync((Jugador?)null);

        // ==================== ACT & ASSERT ====================
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _servicio.ActualizarAsync(jugadorId, new JugadorRequestDto { Nombre = "X", Dorsal = 1 }));
        Assert.Contains("no encontrado", ex.Message);
        _mockJugadorRepo.Verify(r => r.ActualizarAsync(It.IsAny<Jugador>()), Times.Never);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElDorsalYaLoUsaOtroJugador_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int jugadorId = 1;
        int equipoId  = 1;
        var dto = new JugadorRequestDto { Nombre = "Jugador A", Dorsal = 5 }; // dorsal 5 ya lo usa jugador 2

        var jugadorEnBD = new Jugador { Id = jugadorId, Nombre = "Jugador A", Dorsal = 9, EquipoId = equipoId };
        var compañeros = new List<Jugador>
        {
            jugadorEnBD,
            new() { Id = 2, Nombre = "Jugador B", Dorsal = 5, EquipoId = equipoId }
        };

        _mockJugadorRepo.Setup(r => r.ObtenerPorIdAsync(jugadorId)).ReturnsAsync(jugadorEnBD);
        _mockJugadorRepo.Setup(r => r.ObtenerPorEquipoAsync(equipoId)).ReturnsAsync(compañeros);

        // ==================== ACT & ASSERT ====================
        var ex = await Assert.ThrowsAsync<Exception>(() => _servicio.ActualizarAsync(jugadorId, dto));
        Assert.Contains("dorsal", ex.Message);
        _mockJugadorRepo.Verify(r => r.ActualizarAsync(It.IsAny<Jugador>()), Times.Never);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task EliminarAsync_CuandoElJugadorExiste_DeberiaLlamarAlRepositorio()
    {
        // ==================== ARRANGE ====================
        int jugadorId = 1;
        var jugadorEnBD = new Jugador { Id = jugadorId, Nombre = "Jugador A", Dorsal = 9, EquipoId = 1 };
        _mockJugadorRepo.Setup(r => r.ObtenerPorIdAsync(jugadorId)).ReturnsAsync(jugadorEnBD);
        _mockJugadorRepo.Setup(r => r.EliminarAsync(It.IsAny<Jugador>())).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        await _servicio.EliminarAsync(jugadorId);

        // ==================== ASSERT ====================
        _mockJugadorRepo.Verify(r => r.EliminarAsync(jugadorEnBD), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoElJugadorNoExiste_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int jugadorId = 99;
        _mockJugadorRepo.Setup(r => r.ObtenerPorIdAsync(jugadorId)).ReturnsAsync((Jugador?)null);

        // ==================== ACT & ASSERT ====================
        var ex = await Assert.ThrowsAsync<Exception>(() => _servicio.EliminarAsync(jugadorId));
        Assert.Contains("no encontrado", ex.Message);
        _mockJugadorRepo.Verify(r => r.EliminarAsync(It.IsAny<Jugador>()), Times.Never);
    }
}
