using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Tests.Repositories;

public class JugadorRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task AgregarAsync_ConJugadorValido_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);
        
        var equipo = new Equipo { Nombre = "Real Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var jugador = new Jugador { Nombre = "Cristiano", Dorsal = 7, EquipoId = equipo.Id };

        // Act
        await repository.AgregarAsync(jugador);

        // Assert
        var jugadorEnBd = await context.Jugadores.FirstOrDefaultAsync(j => j.Nombre == "Cristiano");
        Assert.NotNull(jugadorEnBd);
        Assert.Equal(7, jugadorEnBd.Dorsal);
    }

    [Fact]
    public async Task ObtenerPorEquipoAsync_ConEquipoIdValido_RetornaListaJugadores()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);
        
        var equipo = new Equipo { Nombre = "Real Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var jugador1 = new Jugador { Nombre = "Cristiano", Dorsal = 7, EquipoId = equipo.Id };
        var jugador2 = new Jugador { Nombre = "Benzema", Dorsal = 9, EquipoId = equipo.Id };
        
        await context.Jugadores.AddRangeAsync(jugador1, jugador2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorEquipoAsync(equipo.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ObtenerPorEquipoAsync_ConEquipoSinJugadores_RetornaListaVacia()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);

        // Act
        var resultado = await repository.ObtenerPorEquipoAsync(999);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaJugador()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);
        
        var equipo = new Equipo { Nombre = "Real Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var jugador = new Jugador { Nombre = "Cristiano", Dorsal = 7, EquipoId = equipo.Id };
        await context.Jugadores.AddAsync(jugador);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(jugador.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Cristiano", resultado.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ActualizarAsync_ConJugadorValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);
        
        var equipo = new Equipo { Nombre = "Real Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var jugador = new Jugador { Nombre = "Cristiano", Dorsal = 7, EquipoId = equipo.Id };
        await context.Jugadores.AddAsync(jugador);
        await context.SaveChangesAsync();

        jugador.Dorsal = 23;

        // Act
        await repository.ActualizarAsync(jugador);

        // Assert
        var jugadorActualizado = await context.Jugadores.FindAsync(jugador.Id);
        Assert.Equal(23, jugadorActualizado.Dorsal);
    }

    [Fact]
    public async Task EliminarAsync_ConJugadorValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new JugadorRepository(context);
        
        var equipo = new Equipo { Nombre = "Real Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var jugador = new Jugador { Nombre = "Cristiano", Dorsal = 7, EquipoId = equipo.Id };
        await context.Jugadores.AddAsync(jugador);
        await context.SaveChangesAsync();

        // Act
        await repository.EliminarAsync(jugador);

        // Assert
        var jugadorEnBd = await context.Jugadores.FindAsync(jugador.Id);
        Assert.Null(jugadorEnBd);
    }
}


