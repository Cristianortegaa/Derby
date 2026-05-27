using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Tests.Repositories;

public class EventoPartidoRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task CrearAsync_ConEventoValido_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);
        
        // Crear dependencias
        var equipo = new Equipo { Nombre = "Equipo 1" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var jugador = new Jugador { Nombre = "Jugador 1", EquipoId = equipo.Id };
        await context.Jugadores.AddAsync(jugador);
        await context.SaveChangesAsync();

        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        var partido = new Partido { EquipoLocalId = equipo.Id, EquipoVisitanteId = equipo.Id, LigaId = liga.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        var evento = new EventoPartido { PartidoId = partido.Id, JugadorId = jugador.Id, Minuto = 10, TipoEvento = TipoEvento.Gol };

        // Act
        var resultado = await repository.CrearAsync(evento);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(TipoEvento.Gol, resultado.TipoEvento);
        var eventoEnBd = await context.EventosPartidos.FirstOrDefaultAsync(e => e.Minuto == 10);
        Assert.NotNull(eventoEnBd);
    }

    [Fact]
    public async Task ObtenerPorPartidoAsync_ConPartidoIdValido_RetornaEventos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);
        
        // Crear dependencias
        var equipo1 = new Equipo { Nombre = "Equipo 1" };
        var equipo2 = new Equipo { Nombre = "Equipo 2" };
        await context.Equipos.AddRangeAsync(equipo1, equipo2);
        await context.SaveChangesAsync();

        var jugador1 = new Jugador { Nombre = "Jugador 1", EquipoId = equipo1.Id };
        var jugador2 = new Jugador { Nombre = "Jugador 2", EquipoId = equipo2.Id };
        await context.Jugadores.AddRangeAsync(jugador1, jugador2);
        await context.SaveChangesAsync();

        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        var partido = new Partido { EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id, LigaId = liga.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        var evento1 = new EventoPartido { PartidoId = partido.Id, JugadorId = jugador1.Id, Minuto = 10, TipoEvento = TipoEvento.Gol };
        var evento2 = new EventoPartido { PartidoId = partido.Id, JugadorId = jugador2.Id, Minuto = 45, TipoEvento = TipoEvento.TarjetaAmarilla };
        
        await context.EventosPartidos.AddRangeAsync(evento1, evento2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorPartidoAsync(partido.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ObtenerPorPartidoAsync_ConPartidoSinEventos_RetornaListaVacia()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);

        // Act
        var resultado = await repository.ObtenerPorPartidoAsync(999);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);
        
        var evento = new EventoPartido { PartidoId = 1, JugadorId = 1, Minuto = 10, TipoEvento = TipoEvento.Gol };
        await context.EventosPartidos.AddAsync(evento);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EliminarAsync(evento.Id);

        // Assert
        Assert.True(resultado);
        var eventoEnBd = await context.EventosPartidos.FindAsync(evento.Id);
        Assert.Null(eventoEnBd);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);

        // Act
        var resultado = await repository.EliminarAsync(999);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ObtenerGolesPorLigaAsync_ConLigaIdValido_RetornaGoles()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);
        
        // Crear equipos
        var equipo1 = new Equipo { Nombre = "Equipo 1" };
        var equipo2 = new Equipo { Nombre = "Equipo 2" };
        await context.Equipos.AddRangeAsync(equipo1, equipo2);
        await context.SaveChangesAsync();

        // Crear jugadores
        var jugador1 = new Jugador { Nombre = "Jugador 1", EquipoId = equipo1.Id };
        var jugador2 = new Jugador { Nombre = "Jugador 2", EquipoId = equipo2.Id };
        await context.Jugadores.AddRangeAsync(jugador1, jugador2);
        await context.SaveChangesAsync();

        // Crear competición
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        // Crear liga
        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        // Crear partido
        var partido = new Partido { EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id, LigaId = liga.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        // Crear eventos
        var evento1 = new EventoPartido { PartidoId = partido.Id, JugadorId = jugador1.Id, TipoEvento = TipoEvento.Gol };
        var evento2 = new EventoPartido { PartidoId = partido.Id, JugadorId = jugador2.Id, TipoEvento = TipoEvento.Gol };
        await context.EventosPartidos.AddRangeAsync(evento1, evento2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerGolesPorLigaAsync(liga.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ObtenerGolesPorCompeticionAsync_ConCompeticionIdValido_RetornaGoles()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new EventoPartidoRepository(context);
        
        // Crear equipos
        var equipo1 = new Equipo { Nombre = "Equipo 1" };
        var equipo2 = new Equipo { Nombre = "Equipo 2" };
        await context.Equipos.AddRangeAsync(equipo1, equipo2);
        await context.SaveChangesAsync();

        // Crear jugadores
        var jugador1 = new Jugador { Nombre = "Jugador 1", EquipoId = equipo1.Id };
        await context.Jugadores.AddAsync(jugador1);
        await context.SaveChangesAsync();

        // Crear competición
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        // Crear liga
        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        // Crear partido
        var partido = new Partido { EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id, LigaId = liga.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        // Crear evento
        var evento = new EventoPartido { PartidoId = partido.Id, JugadorId = jugador1.Id, TipoEvento = TipoEvento.Gol };
        await context.EventosPartidos.AddAsync(evento);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerGolesPorCompeticionAsync(competicion.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
    }
}





