using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class CompeticionServiceTests
{
    private readonly Mock<ICompeticionRepository>   _mockCompRepo    = new();
    private readonly Mock<IPartidoRepository>       _mockPartidoRepo = new();
    private readonly Mock<IEventoPartidoRepository> _mockEventoRepo  = new();
    private readonly CompeticionService             _servicio;

    public CompeticionServiceTests()
    {
        _servicio = new CompeticionService(_mockCompRepo.Object, _mockPartidoRepo.Object, _mockEventoRepo.Object);
    }

    // =========================================================================
    // ObtenerTodas
    // =========================================================================

    [Fact]
    public async Task ObtenerTodasAsync_CuandoHayCompeticiones_DeberiaRetornarLaLista()
    {
        // ==================== ARRANGE ====================
        var competicionesEnBD = new List<Competicion>
        {
            new() { Id = 1, Nombre = "Liga Derby", Temporada = "2025/2026", Estado = "Activo" },
            new() { Id = 2, Nombre = "Copa Derby", Temporada = "2025/2026", Estado = "Activo" },
        };
        _mockCompRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(competicionesEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodasAsync();

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Liga Derby", resultado.First().Nombre);
        _mockCompRepo.Verify(r => r.ObtenerTodasAsync(), Times.Once);
    }

    [Fact]
    public async Task ObtenerTodasAsync_CuandoNoHayCompeticiones_DeberiaRetornarListaVacia()
    {
        // ==================== ARRANGE ====================
        _mockCompRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(new List<Competicion>());

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodasAsync();

        // ==================== ASSERT ====================
        Assert.Empty(resultado);
    }

    // =========================================================================
    // ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoExisteLaCompeticion_DeberiaRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var competicionEnBD = new Competicion { Id = idTest, Nombre = "Liga Derby", Temporada = "2025/2026", Estado = "Activo" };
        _mockCompRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(competicionEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idTest);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal(idTest, resultado!.Id);
        Assert.Equal("Liga Derby", resultado.Nombre);
        _mockCompRepo.Verify(r => r.ObtenerPorIdAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoNoExisteLaCompeticion_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockCompRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Competicion?)null);

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
        var competicionNueva = new Competicion { Nombre = "Nueva Liga", Temporada = "2026/2027" };
        var competicionCreada = new Competicion { Id = 3, Nombre = "Nueva Liga", Temporada = "2026/2027", Estado = "Activo" };
        _mockCompRepo.Setup(r => r.CrearAsync(competicionNueva)).ReturnsAsync(competicionCreada);

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(competicionNueva);

        // ==================== ASSERT ====================
        Assert.Equal(3, resultado.Id);
        Assert.Equal("Nueva Liga", resultado.Nombre);
        _mockCompRepo.Verify(r => r.CrearAsync(competicionNueva), Times.Once);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoLaCompeticionExiste_DeberiaActualizarlaYRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var competicionActualizada = new Competicion { Id = idTest, Nombre = "Actualizada", Temporada = "2026/2027", Estado = "Activo" };
        var competicionParaActualizar = new Competicion { Nombre = "Actualizada", Temporada = "2026/2027" };
        _mockCompRepo.Setup(r => r.ActualizarAsync(idTest, competicionParaActualizar)).ReturnsAsync(competicionActualizada);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, competicionParaActualizar);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal("Actualizada", resultado!.Nombre);
        _mockCompRepo.Verify(r => r.ActualizarAsync(idTest, competicionParaActualizar), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoLaCompeticionNoExiste_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        var comp = new Competicion { Nombre = "X", Temporada = "2026/2027" };
        _mockCompRepo.Setup(r => r.ActualizarAsync(idInexistente, comp)).ReturnsAsync((Competicion?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idInexistente, comp);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task EliminarAsync_CuandoLaCompeticionExiste_DeberiaRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        _mockCompRepo.Setup(r => r.EliminarAsync(idTest)).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado);
        _mockCompRepo.Verify(r => r.EliminarAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoLaCompeticionNoExiste_DeberiaRetornarFalse()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockCompRepo.Setup(r => r.EliminarAsync(idInexistente)).ReturnsAsync(false);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.False(resultado);
    }

    // =========================================================================
    // ObtenerClasificacion — lógica de puntos
    // =========================================================================

    [Fact]
    public async Task ObtenerClasificacionAsync_CuandoUnEquipoGana_DeberiaAsignarTresPuntosAlGanador()
    {
        // ==================== ARRANGE ====================
        var equipoA = new Equipo { Id = 1, Nombre = "Equipo A" };
        var equipoB = new Equipo { Id = 2, Nombre = "Equipo B" };
        var partidos = new List<Partido>
        {
            new() { EquipoLocal = equipoA, EquipoVisitante = equipoB, GolesLocal = 2, GolesVisitante = 0 }
        };
        _mockPartidoRepo.Setup(r => r.ObtenerResultadosAsync(1)).ReturnsAsync(partidos);

        // ==================== ACT ====================
        var clasificacion = await _servicio.ObtenerClasificacionAsync(1);

        // ==================== ASSERT ====================
        Assert.Equal(2, clasificacion.Count);
        Assert.Equal("Equipo A", clasificacion.First().Nombre);  // Primero por puntos
        Assert.Equal(3, clasificacion.First().Puntos);
        Assert.Equal(0, clasificacion.Last().Puntos);
        Assert.Equal(1, clasificacion.First().Ganancias);
        Assert.Equal(1, clasificacion.Last().Derrotas);
        _mockPartidoRepo.Verify(r => r.ObtenerResultadosAsync(1), Times.Once);
    }

    [Fact]
    public async Task ObtenerClasificacionAsync_CuandoHayEmpate_DeberiaAsignarUnPuntoACadaEquipo()
    {
        // ==================== ARRANGE ====================
        var equipoA = new Equipo { Id = 1, Nombre = "Equipo A" };
        var equipoB = new Equipo { Id = 2, Nombre = "Equipo B" };
        var partidos = new List<Partido>
        {
            new() { EquipoLocal = equipoA, EquipoVisitante = equipoB, GolesLocal = 1, GolesVisitante = 1 }
        };
        _mockPartidoRepo.Setup(r => r.ObtenerResultadosAsync(1)).ReturnsAsync(partidos);

        // ==================== ACT ====================
        var clasificacion = await _servicio.ObtenerClasificacionAsync(1);

        // ==================== ASSERT ====================
        Assert.Equal(2, clasificacion.Count);
        Assert.Equal(1, clasificacion[0].Puntos);
        Assert.Equal(1, clasificacion[1].Puntos);
        Assert.Equal(1, clasificacion[0].Empates);
        Assert.Equal(1, clasificacion[1].Empates);
    }

    [Fact]
    public async Task ObtenerClasificacionAsync_SinPartidos_DeberiaRetornarClasificacionVacia()
    {
        // ==================== ARRANGE ====================
        _mockPartidoRepo.Setup(r => r.ObtenerResultadosAsync(1)).ReturnsAsync(new List<Partido>());

        // ==================== ACT ====================
        var clasificacion = await _servicio.ObtenerClasificacionAsync(1);

        // ==================== ASSERT ====================
        Assert.Empty(clasificacion);
    }

    // =========================================================================
    // ObtenerGoleadores
    // =========================================================================

    [Fact]
    public async Task ObtenerGoleadoresAsync_DeberiaAgruparGolesPorJugadorYOrdenarDescendente()
    {
        // ==================== ARRANGE ====================
        var jugador1 = new Jugador { Id = 1, Nombre = "Leo",  Equipo = new Equipo { Nombre = "Equipo A" } };
        var jugador2 = new Jugador { Id = 2, Nombre = "Manu", Equipo = new Equipo { Nombre = "Equipo B" } };
        var eventos = new List<EventoPartido>
        {
            new() { JugadorId = 1, Jugador = jugador1, TipoEvento = TipoEvento.Gol },
            new() { JugadorId = 1, Jugador = jugador1, TipoEvento = TipoEvento.Gol },
            new() { JugadorId = 2, Jugador = jugador2, TipoEvento = TipoEvento.Gol },
        };
        _mockEventoRepo.Setup(r => r.ObtenerGolesPorCompeticionAsync(1)).ReturnsAsync(eventos);

        // ==================== ACT ====================
        var goleadores = await _servicio.ObtenerGoleadoresAsync(1);

        // ==================== ASSERT ====================
        Assert.Equal(2, goleadores.Count);
        Assert.Equal("Leo", goleadores.First().Nombre);   // Más goles → primero
        Assert.Equal(2, goleadores.First().Goles);
        Assert.Equal(1, goleadores.Last().Goles);
        _mockEventoRepo.Verify(r => r.ObtenerGolesPorCompeticionAsync(1), Times.Once);
    }
}
