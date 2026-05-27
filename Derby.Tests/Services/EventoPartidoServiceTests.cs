using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class EventoPartidoServiceTests
{
    private readonly Mock<IEventoPartidoRepository> _mockEventoRepo  = new();
    private readonly Mock<IPartidoRepository>       _mockPartidoRepo = new();
    private readonly EventoPartidoService           _servicio;

    public EventoPartidoServiceTests()
    {
        _servicio = new EventoPartidoService(_mockEventoRepo.Object, _mockPartidoRepo.Object);
    }

    // =========================================================================
    // ObtenerEventos
    // =========================================================================

    [Fact]
    public async Task ObtenerEventosAsync_CuandoHayEventos_DeberiaRetornarLaLista()
    {
        // ==================== ARRANGE ====================
        int partidoId = 1;
        var eventosEnBD = new List<EventoPartido>
        {
            new() { Id = 1, PartidoId = partidoId, JugadorId = 1, Minuto = 10, TipoEvento = TipoEvento.Gol,
                    Jugador = new Jugador { Id = 1, Nombre = "Jugador A", Dorsal = 9, EquipoId = 1 } },
            new() { Id = 2, PartidoId = partidoId, JugadorId = 2, Minuto = 55, TipoEvento = TipoEvento.TarjetaAmarilla,
                    Jugador = new Jugador { Id = 2, Nombre = "Jugador B", Dorsal = 5, EquipoId = 2 } }
        };
        _mockEventoRepo.Setup(r => r.ObtenerPorPartidoAsync(partidoId)).ReturnsAsync(eventosEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerEventosAsync(partidoId);

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        Assert.Equal(10, resultado[0].Minuto);
        Assert.Equal("Gol", resultado[0].TipoEvento);
        _mockEventoRepo.Verify(r => r.ObtenerPorPartidoAsync(partidoId), Times.Once);
    }

    [Fact]
    public async Task ObtenerEventosAsync_CuandoNoHayEventos_DeberiaRetornarListaVacia()
    {
        // ==================== ARRANGE ====================
        int partidoId = 99;
        _mockEventoRepo.Setup(r => r.ObtenerPorPartidoAsync(partidoId)).ReturnsAsync(new List<EventoPartido>());

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerEventosAsync(partidoId);

        // ==================== ASSERT ====================
        Assert.Empty(resultado);
    }

    // =========================================================================
    // AñadirEvento
    // =========================================================================

    [Fact]
    public async Task AñadirEventoAsync_CuandoElTipoEsValido_DeberiaCrearElEventoYRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int partidoId = 1;
        var dto = new EventoPartidoRequestDto { JugadorId = 3, Minuto = 25, TipoEvento = "Gol" };

        var eventoCreado = new EventoPartido
        {
            Id = 10, PartidoId = partidoId, JugadorId = 3, Minuto = 25, TipoEvento = TipoEvento.Gol,
            Jugador = new Jugador { Id = 3, Nombre = "Jugador C", Dorsal = 10, EquipoId = 1 }
        };
        _mockEventoRepo.Setup(r => r.CrearAsync(It.IsAny<EventoPartido>())).ReturnsAsync(eventoCreado);

        // ==================== ACT ====================
        var resultado = await _servicio.AñadirEventoAsync(partidoId, dto);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal(25, resultado!.Minuto);
        Assert.Equal("Gol", resultado.TipoEvento);
        _mockEventoRepo.Verify(r => r.CrearAsync(It.IsAny<EventoPartido>()), Times.Once);
    }

    [Fact]
    public async Task AñadirEventoAsync_CuandoElTipoEsInvalido_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        var dto = new EventoPartidoRequestDto { JugadorId = 1, Minuto = 10, TipoEvento = "TipoInexistente" };

        // ==================== ACT ====================
        var resultado = await _servicio.AñadirEventoAsync(1, dto);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
        _mockEventoRepo.Verify(r => r.CrearAsync(It.IsAny<EventoPartido>()), Times.Never);
    }

    // =========================================================================
    // EliminarEvento
    // =========================================================================

    [Fact]
    public async Task EliminarEventoAsync_CuandoElEventoExiste_DeberiaRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int eventoId = 1;
        _mockEventoRepo.Setup(r => r.EliminarAsync(eventoId)).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarEventoAsync(eventoId);

        // ==================== ASSERT ====================
        Assert.True(resultado);
        _mockEventoRepo.Verify(r => r.EliminarAsync(eventoId), Times.Once);
    }

    [Fact]
    public async Task EliminarEventoAsync_CuandoElEventoNoExiste_DeberiaRetornarFalse()
    {
        // ==================== ARRANGE ====================
        int eventoId = 99;
        _mockEventoRepo.Setup(r => r.EliminarAsync(eventoId)).ReturnsAsync(false);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarEventoAsync(eventoId);

        // ==================== ASSERT ====================
        Assert.False(resultado);
    }

    // =========================================================================
    // CerrarActa
    // =========================================================================

    [Fact]
    public async Task CerrarActaAsync_CuandoElPartidoNoExiste_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int partidoId = 99;
        _mockPartidoRepo.Setup(r => r.ObtenerPorIdAsync(partidoId)).ReturnsAsync((Partido?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.CerrarActaAsync(partidoId);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
        _mockPartidoRepo.Verify(r => r.FinalizarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CerrarActaAsync_DeberiaContarGolesCorrectamenteYFinalizarElPartido()
    {
        // ==================== ARRANGE ====================
        int partidoId = 1;
        int equipoLocalId = 10;
        int equipoVisitanteId = 20;

        var partido = new Partido
        {
            Id = partidoId,
            EquipoLocalId = equipoLocalId,
            EquipoVisitanteId = equipoVisitanteId
        };

        // 2 goles del local, 1 del visitante, 1 tarjeta (no cuenta)
        var eventos = new List<EventoPartido>
        {
            new() { TipoEvento = TipoEvento.Gol,            Jugador = new Jugador { EquipoId = equipoLocalId } },
            new() { TipoEvento = TipoEvento.Gol,            Jugador = new Jugador { EquipoId = equipoLocalId } },
            new() { TipoEvento = TipoEvento.Gol,            Jugador = new Jugador { EquipoId = equipoVisitanteId } },
            new() { TipoEvento = TipoEvento.TarjetaAmarilla, Jugador = new Jugador { EquipoId = equipoLocalId } },
        };

        var partidoFinalizado = new Partido { Id = partidoId, GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado" };

        _mockPartidoRepo.Setup(r => r.ObtenerPorIdAsync(partidoId)).ReturnsAsync(partido);
        _mockEventoRepo.Setup(r => r.ObtenerPorPartidoAsync(partidoId)).ReturnsAsync(eventos);
        _mockPartidoRepo.Setup(r => r.FinalizarAsync(partidoId, 2, 1)).ReturnsAsync(partidoFinalizado);

        // ==================== ACT ====================
        var resultado = await _servicio.CerrarActaAsync(partidoId);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado!.GolesLocal);
        Assert.Equal(1, resultado.GolesVisitante);
        _mockPartidoRepo.Verify(r => r.FinalizarAsync(partidoId, 2, 1), Times.Once);
    }
}
