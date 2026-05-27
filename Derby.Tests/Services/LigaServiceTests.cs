using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class LigaServiceTests
{
    private readonly Mock<ILigaRepository>         _mockLigaRepo    = new();
    private readonly Mock<IPartidoRepository>       _mockPartidoRepo = new();
    private readonly Mock<IEventoPartidoRepository> _mockEventoRepo  = new();
    private readonly LigaService                   _servicio;

    public LigaServiceTests()
    {
        _servicio = new LigaService(_mockLigaRepo.Object, _mockPartidoRepo.Object, _mockEventoRepo.Object);
    }

    // =========================================================================
    // ObtenerTodas / ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerTodasAsync_CuandoHayLigas_DeberiaRetornarLaLista()
    {
        // ==================== ARRANGE ====================
        var ligasEnBD = new List<Liga>
        {
            new() { Id = 1, Nombre = "Primera DAW", CompeticionId = 1, Jornadas = 10, JornadaActual = 0, Estado = "Activo", Grupo = "Único" },
            new() { Id = 2, Nombre = "Segunda DAW", CompeticionId = 1, Jornadas = 10, JornadaActual = 0, Estado = "Activo", Grupo = "Único" },
        };
        _mockLigaRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(ligasEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodasAsync();

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        Assert.Equal("Primera DAW", resultado.First().Nombre);
        _mockLigaRepo.Verify(r => r.ObtenerTodasAsync(), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoLaLigaExiste_DeberiaRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var ligaEnBD = new Liga { Id = idTest, Nombre = "Primera DAW", CompeticionId = 1, Jornadas = 10, JornadaActual = 2, Estado = "Activo", Grupo = "Único" };
        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(ligaEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idTest);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado!.JornadaActual);
        _mockLigaRepo.Verify(r => r.ObtenerPorIdAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoLaLigaNoExiste_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Liga?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
    }

    // =========================================================================
    // Crear / Eliminar
    // =========================================================================

    [Fact]
    public async Task CrearAsync_DeberiaLlamarAlRepositorioYRetornarElDto()
    {
        // ==================== ARRANGE ====================
        var dto = new LigaRequestDto { Nombre = "Nueva Liga", CompeticionId = 1, Grupo = "Único", Jornadas = 10, JornadaActual = 0, Estado = "Activo" };
        var ligaCreada = new Liga { Id = 5, Nombre = "Nueva Liga", CompeticionId = 1, Grupo = "Único", Jornadas = 10, JornadaActual = 0, Estado = "Activo" };
        _mockLigaRepo.Setup(r => r.CrearAsync(It.IsAny<Liga>())).ReturnsAsync(ligaCreada);

        // ==================== ACT ====================
        var resultado = await _servicio.CrearAsync(dto);

        // ==================== ASSERT ====================
        Assert.Equal(5, resultado.Id);
        Assert.Equal("Nueva Liga", resultado.Nombre);
        _mockLigaRepo.Verify(r => r.CrearAsync(It.IsAny<Liga>()), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoLaLigaExiste_DeberiaRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        _mockLigaRepo.Setup(r => r.EliminarAsync(idTest)).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado);
        _mockLigaRepo.Verify(r => r.EliminarAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoLaLigaNoExiste_DeberiaRetornarFalse()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockLigaRepo.Setup(r => r.EliminarAsync(idInexistente)).ReturnsAsync(false);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.False(resultado);
    }

    // =========================================================================
    // AgregarEquipo — validaciones de negocio
    // =========================================================================

    [Fact]
    public async Task AgregarEquipoAsync_CuandoElEquipoYaEstaEnLaLiga_DeberiaLanzarExceptionYNOAgregar()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1, equipoId = 5;
        _mockLigaRepo.Setup(r => r.EquipoExisteAsync(ligaId, equipoId)).ReturnsAsync(true);

        // ==================== ACT & ASSERT ====================
        var excepcion = await Assert.ThrowsAsync<Exception>(() => _servicio.AgregarEquipoAsync(ligaId, equipoId));
        Assert.Contains("ya está en esta liga", excepcion.Message);

        // Nunca debe llegar a agregar
        _mockLigaRepo.Verify(r => r.AgregarEquipoAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AgregarEquipoAsync_CuandoLaLigaTieneMasDe20Equipos_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1, equipoId = 5;
        _mockLigaRepo.Setup(r => r.EquipoExisteAsync(ligaId, equipoId)).ReturnsAsync(false);

        // Simulamos 20 equipos en la liga
        var equiposEnLiga = Enumerable.Range(1, 20).Select(i => new Equipo { Id = i, Nombre = $"Equipo {i}" }).ToList();
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(equiposEnLiga);

        // ==================== ACT & ASSERT ====================
        var excepcion = await Assert.ThrowsAsync<Exception>(() => _servicio.AgregarEquipoAsync(ligaId, equipoId));
        Assert.Contains("máximo de 20 equipos", excepcion.Message);
        _mockLigaRepo.Verify(r => r.AgregarEquipoAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AgregarEquipoAsync_CuandoElEquipoYaPerteneceAOtraLiga_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1, equipoId = 5;
        _mockLigaRepo.Setup(r => r.EquipoExisteAsync(ligaId, equipoId)).ReturnsAsync(false);
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo> { new() { Id = 1 } });

        // El equipo 5 NO aparece en "sin liga" → ya tiene liga
        _mockLigaRepo.Setup(r => r.ObtenerEquiposSinLigaAsync()).ReturnsAsync(new List<Equipo> { new() { Id = 9 } });

        // ==================== ACT & ASSERT ====================
        var excepcion = await Assert.ThrowsAsync<Exception>(() => _servicio.AgregarEquipoAsync(ligaId, equipoId));
        Assert.Contains("ya pertenece a otra liga", excepcion.Message);
        _mockLigaRepo.Verify(r => r.AgregarEquipoAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AgregarEquipoAsync_CuandoTodoEsValido_DeberiaLlamarAlRepositorioExactamenteUnaVez()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1, equipoId = 5;
        _mockLigaRepo.Setup(r => r.EquipoExisteAsync(ligaId, equipoId)).ReturnsAsync(false);
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo> { new() { Id = 1 } });
        _mockLigaRepo.Setup(r => r.ObtenerEquiposSinLigaAsync()).ReturnsAsync(new List<Equipo> { new() { Id = equipoId, Nombre = "E5" } });
        _mockLigaRepo.Setup(r => r.AgregarEquipoAsync(ligaId, equipoId)).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        await _servicio.AgregarEquipoAsync(ligaId, equipoId);

        // ==================== ASSERT ====================
        _mockLigaRepo.Verify(r => r.AgregarEquipoAsync(ligaId, equipoId), Times.Once);
    }

    // =========================================================================
    // GenerarCalendario
    // =========================================================================

    [Fact]
    public async Task GenerarCalendarioAsync_CuandoLaLigaNoExiste_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Liga?)null);

        // ==================== ACT & ASSERT ====================
        var excepcion = await Assert.ThrowsAsync<Exception>(() => _servicio.GenerarCalendarioAsync(idInexistente));
        Assert.Contains("Liga no encontrada", excepcion.Message);

        // Nunca debe intentar crear partidos
        _mockPartidoRepo.Verify(r => r.CrearRangoAsync(It.IsAny<List<Partido>>()), Times.Never);
    }

    [Fact]
    public async Task GenerarCalendarioAsync_CuandoHayMenosDeDosEquipos_DeberiaLanzarException()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
        _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo> { new() { Id = 1, Nombre = "Solo" } });

        // ==================== ACT & ASSERT ====================
        var excepcion = await Assert.ThrowsAsync<Exception>(() => _servicio.GenerarCalendarioAsync(ligaId));
        Assert.Contains("al menos 2 equipos", excepcion.Message);
        _mockPartidoRepo.Verify(r => r.CrearRangoAsync(It.IsAny<List<Partido>>()), Times.Never);
    }

    [Fact]
    public async Task GenerarCalendarioAsync_ConDosEquipos_DeberiaCrearDosPartidosIdaYVuelta()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
        _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo>
        {
            new() { Id = 1, Nombre = "A" },
            new() { Id = 2, Nombre = "B" }
        });
        _mockPartidoRepo.Setup(r => r.CrearRangoAsync(It.IsAny<List<Partido>>())).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>())).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        var resultado = await _servicio.GenerarCalendarioAsync(ligaId);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);

        // Con 2 equipos → 2 partidos (ida y vuelta)
        _mockPartidoRepo.Verify(r => r.CrearRangoAsync(It.Is<List<Partido>>(l => l.Count == 2)), Times.Once);
        _mockLigaRepo.Verify(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>()), Times.Once);
    }

     // =========================================================================
     // GenerarCalendario — casos adicionales con múltiples equipos
     // =========================================================================

     [Fact]
     public async Task GenerarCalendarioAsync_ConTresEquipos_DeberíaGenerarCalendario()
     {
         // ==================== ARRANGE ====================
         int ligaId = 1;
         var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

         _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
         _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
         _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo>
         {
             new() { Id = 1, Nombre = "A" },
             new() { Id = 2, Nombre = "B" },
             new() { Id = 3, Nombre = "C" }
         });
         _mockPartidoRepo.Setup(r => r.CrearRangoAsync(It.IsAny<List<Partido>>())).Returns(Task.CompletedTask);
         _mockLigaRepo.Setup(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>())).Returns(Task.CompletedTask);

         // ==================== ACT ====================
         var resultado = await _servicio.GenerarCalendarioAsync(ligaId);

         // ==================== ASSERT ====================
         Assert.NotNull(resultado);
         _mockPartidoRepo.Verify(r => r.CrearRangoAsync(It.IsAny<List<Partido>>()), Times.Once);
     }

     [Fact]
     public async Task GenerarCalendarioAsync_ConCuatroEquipos_DeberíaGenerarCalendario()
     {
         // ==================== ARRANGE ====================
         int ligaId = 1;
         var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

         _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
         _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
         _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo>
         {
             new() { Id = 1, Nombre = "A" },
             new() { Id = 2, Nombre = "B" },
             new() { Id = 3, Nombre = "C" },
             new() { Id = 4, Nombre = "D" }
         });
         _mockPartidoRepo.Setup(r => r.CrearRangoAsync(It.IsAny<List<Partido>>())).Returns(Task.CompletedTask);
         _mockLigaRepo.Setup(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>())).Returns(Task.CompletedTask);

         // ==================== ACT ====================
         var resultado = await _servicio.GenerarCalendarioAsync(ligaId);

         // ==================== ASSERT ====================
         Assert.NotNull(resultado);
         _mockPartidoRepo.Verify(r => r.CrearRangoAsync(It.IsAny<List<Partido>>()), Times.Once);
     }

     [Fact]
     public async Task GenerarCalendarioAsync_ConSeisEquipos_DeberíaGenerarCalendario()
     {
         // ==================== ARRANGE ====================
         int ligaId = 1;
         var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

         _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
         _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
         _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo>
         {
             new() { Id = 1, Nombre = "A" },
             new() { Id = 2, Nombre = "B" },
             new() { Id = 3, Nombre = "C" },
             new() { Id = 4, Nombre = "D" },
             new() { Id = 5, Nombre = "E" },
             new() { Id = 6, Nombre = "F" }
         });
         _mockPartidoRepo.Setup(r => r.CrearRangoAsync(It.IsAny<List<Partido>>())).Returns(Task.CompletedTask);
         _mockLigaRepo.Setup(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>())).Returns(Task.CompletedTask);

         // ==================== ACT ====================
         var resultado = await _servicio.GenerarCalendarioAsync(ligaId);

         // ==================== ASSERT ====================
         Assert.NotNull(resultado);
         _mockPartidoRepo.Verify(r => r.CrearRangoAsync(It.IsAny<List<Partido>>()), Times.Once);
     }


    [Fact]
    public async Task GenerarCalendarioAsync_DeberiaRetornarObjetoConPropiedadesCorrecto()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
        _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo>
        {
            new() { Id = 1, Nombre = "A" },
            new() { Id = 2, Nombre = "B" }
        });
        _mockPartidoRepo.Setup(r => r.CrearRangoAsync(It.IsAny<List<Partido>>())).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>())).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        var resultado = await _servicio.GenerarCalendarioAsync(ligaId);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        // Verificar que devuelve un objeto anónimo con las propiedades correctas
        var resultType = resultado.GetType();
        Assert.NotNull(resultType.GetProperty("mensaje"));
        Assert.NotNull(resultType.GetProperty("jornadas"));
        Assert.NotNull(resultType.GetProperty("totalPartidos"));
    }

    [Fact]
    public async Task GenerarCalendarioAsync_DeberiaLlamarActualizarJornadasConValorCorrecto()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var liga = new Liga { Id = ligaId, Nombre = "L", CompeticionId = 1, Jornadas = 0, JornadaActual = 0, Estado = "Activo", Grupo = "Único" };

        _mockLigaRepo.Setup(r => r.ObtenerPorIdAsync(ligaId)).ReturnsAsync(liga);
        _mockPartidoRepo.Setup(r => r.EliminarPorLigaAsync(ligaId)).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(new List<Equipo>
        {
            new() { Id = 1, Nombre = "A" },
            new() { Id = 2, Nombre = "B" },
            new() { Id = 3, Nombre = "C" },
            new() { Id = 4, Nombre = "D" }
        });
        _mockPartidoRepo.Setup(r => r.CrearRangoAsync(It.IsAny<List<Partido>>())).Returns(Task.CompletedTask);
        _mockLigaRepo.Setup(r => r.ActualizarJornadasAsync(ligaId, It.IsAny<int>())).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        await _servicio.GenerarCalendarioAsync(ligaId);

        // ==================== ASSERT ====================
        // Con 4 equipos → (4-1)*2 = 6 jornadas
        _mockLigaRepo.Verify(r => r.ActualizarJornadasAsync(ligaId, 6), Times.Once);
    }

    // =========================================================================
    // ObtenerClasificacion
    // =========================================================================

    [Fact]
    public async Task ObtenerClasificacionAsync_CuandoGanaElLocal_DeberiaAsignarle3PuntosYOrdenarCorrectamente()
    {
        // ==================== ARRANGE ====================
        var equipoA = new Equipo { Id = 1, Nombre = "Equipo A" };
        var equipoB = new Equipo { Id = 2, Nombre = "Equipo B" };
        var partidos = new List<Partido>
        {
            new() { EquipoLocal = equipoA, EquipoVisitante = equipoB, GolesLocal = 3, GolesVisitante = 0 }
        };
        _mockPartidoRepo.Setup(r => r.ObtenerResultadosPorLigaAsync(1)).ReturnsAsync(partidos);

        // ==================== ACT ====================
        var clasificacion = await _servicio.ObtenerClasificacionAsync(1);

        // ==================== ASSERT ====================
        Assert.Equal(2, clasificacion.Count);
        Assert.Equal("Equipo A", clasificacion.First().Nombre);
        Assert.Equal(3, clasificacion.First().Puntos);
        Assert.Equal(1, clasificacion.First().Ganancias);
        Assert.Equal(1, clasificacion.Last().Derrotas);
        Assert.Equal(0, clasificacion.Last().Puntos);
        _mockPartidoRepo.Verify(r => r.ObtenerResultadosPorLigaAsync(1), Times.Once);
    }

    // =========================================================================
    // ObtenerGoleadores
    // =========================================================================

    [Fact]
    public async Task ObtenerGoleadoresAsync_DeberiaAgruparGolesPorJugadorYOrdenarDescendente()
    {
        // ==================== ARRANGE ====================
        var jugador = new Jugador { Id = 1, Nombre = "Leo", Equipo = new Equipo { Nombre = "Equipo A" } };
        var eventos = new List<EventoPartido>
        {
            new() { JugadorId = 1, Jugador = jugador, TipoEvento = TipoEvento.Gol },
            new() { JugadorId = 1, Jugador = jugador, TipoEvento = TipoEvento.Gol },
            new() { JugadorId = 1, Jugador = jugador, TipoEvento = TipoEvento.Gol },
        };
        _mockEventoRepo.Setup(r => r.ObtenerGolesPorLigaAsync(1)).ReturnsAsync(eventos);

        // ==================== ACT ====================
        var goleadores = await _servicio.ObtenerGoleadoresAsync(1);

        // ==================== ASSERT ====================
        Assert.Single(goleadores);
        Assert.Equal(3, goleadores.First().Goles);
        Assert.Equal("Leo", goleadores.First().Nombre);
        _mockEventoRepo.Verify(r => r.ObtenerGolesPorLigaAsync(1), Times.Once);
    }

    // =========================================================================
    // ActualizarAsync
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoLaLigaExiste_DeberiaRetornarElDto()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var dto = new LigaRequestDto { Nombre = "Primera DAW Actualizada", CompeticionId = 1, Grupo = "Único", Jornadas = 14, JornadaActual = 3, Estado = "Activo" };
        var ligaActualizada = new Liga { Id = idTest, Nombre = "Primera DAW Actualizada", CompeticionId = 1, Grupo = "Único", Jornadas = 14, JornadaActual = 3, Estado = "Activo" };
        _mockLigaRepo.Setup(r => r.ActualizarAsync(idTest, It.IsAny<Liga>())).ReturnsAsync(ligaActualizada);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.NotNull(resultado);
        Assert.Equal("Primera DAW Actualizada", resultado!.Nombre);
        _mockLigaRepo.Verify(r => r.ActualizarAsync(idTest, It.IsAny<Liga>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoLaLigaNoExiste_DeberiaRetornarNull()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        var dto = new LigaRequestDto { Nombre = "X", CompeticionId = 1, Grupo = "Único", Jornadas = 10, JornadaActual = 0, Estado = "Activo" };
        _mockLigaRepo.Setup(r => r.ActualizarAsync(idInexistente, It.IsAny<Liga>())).ReturnsAsync((Liga?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idInexistente, dto);

        // ==================== ASSERT ====================
        Assert.Null(resultado);
    }

    // =========================================================================
    // ObtenerEquipos / ObtenerEquiposSinLiga / QuitarEquipo
    // =========================================================================

    [Fact]
    public async Task ObtenerEquiposAsync_DeberiaRetornarLosEquiposDeLaLiga()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var equipos = new List<Equipo>
        {
            new() { Id = 1, Nombre = "FC Derby Norte", Sede = "Estadio Norte", Entrenador = "Luis" },
            new() { Id = 2, Nombre = "Atlético Sur CF", Sede = "Estadio Sur",  Entrenador = "Pedro" }
        };
        _mockLigaRepo.Setup(r => r.ObtenerEquiposAsync(ligaId)).ReturnsAsync(equipos);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerEquiposAsync(ligaId);

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        _mockLigaRepo.Verify(r => r.ObtenerEquiposAsync(ligaId), Times.Once);
    }

    [Fact]
    public async Task ObtenerEquiposSinLigaAsync_DeberiaRetornarEquiposNoAsignados()
    {
        // ==================== ARRANGE ====================
        var equiposSinLiga = new List<Equipo>
        {
            new() { Id = 5, Nombre = "Equipo Libre", Sede = "Campo Libre", Entrenador = "Carlos" }
        };
        _mockLigaRepo.Setup(r => r.ObtenerEquiposSinLigaAsync()).ReturnsAsync(equiposSinLiga);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerEquiposSinLigaAsync();

        // ==================== ASSERT ====================
        Assert.Single(resultado);
        Assert.Equal("Equipo Libre", resultado[0].Nombre);
        _mockLigaRepo.Verify(r => r.ObtenerEquiposSinLigaAsync(), Times.Once);
    }

    [Fact]
    public async Task QuitarEquipoAsync_DeberiaLlamarAlRepositorio()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1, equipoId = 2;
        _mockLigaRepo.Setup(r => r.QuitarEquipoAsync(ligaId, equipoId)).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        await _servicio.QuitarEquipoAsync(ligaId, equipoId);

        // ==================== ASSERT ====================
        _mockLigaRepo.Verify(r => r.QuitarEquipoAsync(ligaId, equipoId), Times.Once);
    }

    // =========================================================================
    // ObtenerJornadas / ObtenerResultados
    // =========================================================================

    [Fact]
    public async Task ObtenerJornadasAsync_DeberiaAgruparPartidosPorJornada()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var partidos = new List<Partido>
        {
            new() { Id = 1, Jornada = 1, LigaId = ligaId, EquipoLocalId = 1, EquipoVisitanteId = 2, FechaHora = DateTime.Now, Estado = "Pendiente" },
            new() { Id = 2, Jornada = 1, LigaId = ligaId, EquipoLocalId = 3, EquipoVisitanteId = 4, FechaHora = DateTime.Now, Estado = "Pendiente" },
            new() { Id = 3, Jornada = 2, LigaId = ligaId, EquipoLocalId = 1, EquipoVisitanteId = 3, FechaHora = DateTime.Now, Estado = "Pendiente" },
        };
        _mockPartidoRepo.Setup(r => r.ObtenerPorLigaAsync(ligaId)).ReturnsAsync(partidos);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerJornadasAsync(ligaId);

        // ==================== ASSERT ====================
        Assert.Equal(2, resultado.Count);
        Assert.Equal(1, resultado[0].Numero);
        Assert.Equal(2, resultado[0].Partidos.Count);
        Assert.Equal(2, resultado[1].Numero);
        Assert.Single(resultado[1].Partidos);
    }

    [Fact]
    public async Task ObtenerResultadosAsync_DeberiaRetornarSoloPartidosFinalizados()
    {
        // ==================== ARRANGE ====================
        int ligaId = 1;
        var partidos = new List<Partido>
        {
            new() { Id = 1, GolesLocal = 2, GolesVisitante = 1, Estado = "Finalizado",
                    EquipoLocal = new Equipo { Id = 1, Nombre = "Local",    Sede = "S1", Entrenador = "E1" },
                    EquipoVisitante = new Equipo { Id = 2, Nombre = "Visitante", Sede = "S2", Entrenador = "E2" } }
        };
        _mockPartidoRepo.Setup(r => r.ObtenerResultadosPorLigaAsync(ligaId)).ReturnsAsync(partidos);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerResultadosAsync(ligaId);

        // ==================== ASSERT ====================
        Assert.Single(resultado);
        Assert.Equal(2, resultado[0].GolesLocal);
        Assert.Equal(1, resultado[0].GolesVisitante);
        _mockPartidoRepo.Verify(r => r.ObtenerResultadosPorLigaAsync(ligaId), Times.Once);
    }
}
