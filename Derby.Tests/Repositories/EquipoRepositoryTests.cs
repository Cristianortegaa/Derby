using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Derby.Tests.Repositories;

public class EquipoRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task CrearAsync_ConEquipoValido_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);
        var equipo = new Equipo { Nombre = "Real Madrid", Entrenador = "Carlo", Sede = "Madrid" };

        // Act
        var resultado = await repository.CrearAsync(equipo);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Real Madrid", resultado.Nombre);
        var equipoEnBd = await context.Equipos.FirstOrDefaultAsync(e => e.Nombre == "Real Madrid");
        Assert.NotNull(equipoEnBd);
    }

    [Fact]
    public async Task ObtenerTodosAsync_RetornaListaDeEquipos()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);
        
        var equipo1 = new Equipo { Nombre = "Real Madrid", Entrenador = "Carlo", Sede = "Madrid" };
        var equipo2 = new Equipo { Nombre = "Barcelona", Entrenador = "Flick", Sede = "Barcelona" };
        
        await context.Equipos.AddRangeAsync(equipo1, equipo2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaEquipo()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);
        
        var equipo = new Equipo { Nombre = "Real Madrid", Entrenador = "Carlo", Sede = "Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(equipo.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Real Madrid", resultado.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ActualizarAsync_ConEquipoValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);
        
        var equipo = new Equipo { Nombre = "Real Madrid", Entrenador = "Carlo", Sede = "Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        equipo.Entrenador = "Ancelotti";

        // Act
        var resultado = await repository.ActualizarAsync(equipo);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Ancelotti", resultado.Entrenador);
        
        var equipoActualizado = await context.Equipos.FindAsync(equipo.Id);
        Assert.Equal("Ancelotti", equipoActualizado.Entrenador);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);
        
        var equipo = new Equipo { Nombre = "Real Madrid", Entrenador = "Carlo", Sede = "Madrid" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EliminarAsync(equipo.Id);

        // Assert
        Assert.True(resultado);
        var equipoEnBd = await context.Equipos.FindAsync(equipo.Id);
        Assert.Null(equipoEnBd);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<EquipoRepository>>();
        var repository = new EquipoRepository(context, logger);

        // Act
        var resultado = await repository.EliminarAsync(999);

        // Assert
        Assert.False(resultado);
    }
}


