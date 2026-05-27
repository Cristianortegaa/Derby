using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Tests.Repositories;

public class PartidoRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    private async Task<(Liga Liga, Equipo EquipoLocal, Equipo EquipoVisitante)> SetupLigaYEquiposAsync(DerbyContext context)
    {
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var equipo1 = new Equipo { Nombre = "Equipo 1" };
        var equipo2 = new Equipo { Nombre = "Equipo 2" };
        await context.Equipos.AddRangeAsync(equipo1, equipo2);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        return (liga, equipo1, equipo2);
    }

    [Fact]
    public async Task CrearAsync_ConPartidoValido_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);
        
        var (liga, equipo1, equipo2) = await SetupLigaYEquiposAsync(context);
        var partido = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id };

        // Act
        var resultado = await repository.CrearAsync(partido);

        // Assert
        Assert.NotNull(resultado);
        var partidoEnBd = await context.Partidos.FirstOrDefaultAsync();
        Assert.NotNull(partidoEnBd);
    }

    [Fact]
    public async Task ObtenerTodosAsync_RetornaListaDePartidos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);
        
        var (liga, equipo1, equipo2) = await SetupLigaYEquiposAsync(context);
        
        var partido1 = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id };
        var partido2 = new Partido { LigaId = liga.Id, EquipoLocalId = equipo2.Id, EquipoVisitanteId = equipo1.Id };
        
        await context.Partidos.AddRangeAsync(partido1, partido2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaPartido()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);
        
        var (liga, equipo1, equipo2) = await SetupLigaYEquiposAsync(context);
        var partido = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(partido.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(partido.Id, resultado.Id);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerPorLigaAsync_ConLigaIdValido_RetornaPartidos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);
        
        var (liga, equipo1, equipo2) = await SetupLigaYEquiposAsync(context);
        
        var partido1 = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id };
        var partido2 = new Partido { LigaId = liga.Id, EquipoLocalId = equipo2.Id, EquipoVisitanteId = equipo1.Id };
        
        await context.Partidos.AddRangeAsync(partido1, partido2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorLigaAsync(liga.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ActualizarAsync_ConPartidoValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);
        
        var (liga, equipo1, equipo2) = await SetupLigaYEquiposAsync(context);
        var partido = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        var partidoActualizar = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id, GolesLocal = 2, GolesVisitante = 1 };

        // Act
        var resultado = await repository.ActualizarAsync(partido.Id, partidoActualizar);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.GolesLocal);
        
        var partidoActualizado = await context.Partidos.FindAsync(partido.Id);
        Assert.Equal(2, partidoActualizado.GolesLocal);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);
        
        var (liga, equipo1, equipo2) = await SetupLigaYEquiposAsync(context);
        var partido = new Partido { LigaId = liga.Id, EquipoLocalId = equipo1.Id, EquipoVisitanteId = equipo2.Id };
        await context.Partidos.AddAsync(partido);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EliminarAsync(partido.Id);

        // Assert
        Assert.True(resultado);
        var partidoEnBd = await context.Partidos.FindAsync(partido.Id);
        Assert.Null(partidoEnBd);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new PartidoRepository(context);

        // Act
        var resultado = await repository.EliminarAsync(999);

        // Assert
        Assert.False(resultado);
    }
}




