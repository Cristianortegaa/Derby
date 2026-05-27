using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Tests.Repositories;

public class LigaRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task CrearAsync_ConLigaValida_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };

        // Act
        var resultado = await repository.CrearAsync(liga);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 1", resultado.Nombre);
        var ligaEnBd = await context.Ligas.FirstOrDefaultAsync(l => l.Nombre == "Liga 1");
        Assert.NotNull(ligaEnBd);
    }

    [Fact]
    public async Task ObtenerTodasAsync_RetornaListaDeLingas()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga1 = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        var liga2 = new Liga { Nombre = "Liga 2", CompeticionId = competicion.Id };
        
        await context.Ligas.AddRangeAsync(liga1, liga2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerTodasAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaLiga()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(liga.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 1", resultado.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerEquiposAsync_ConLigaIdValido_RetornaEquipos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
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

        var ligaEquipo1 = new LigaEquipo { LigaId = liga.Id, EquipoId = equipo1.Id };
        var ligaEquipo2 = new LigaEquipo { LigaId = liga.Id, EquipoId = equipo2.Id };
        await context.LigaEquipos.AddRangeAsync(ligaEquipo1, ligaEquipo2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerEquiposAsync(liga.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task AgregarEquipoAsync_ConEquipoValido_AgregarEquipoALiga()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var equipo = new Equipo { Nombre = "Equipo 1" };
        await context.Equipos.AddAsync(equipo);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        // Act
        await repository.AgregarEquipoAsync(liga.Id, equipo.Id);

        // Assert
        var ligaEquipo = await context.LigaEquipos.FirstOrDefaultAsync(le => le.LigaId == liga.Id && le.EquipoId == equipo.Id);
        Assert.NotNull(ligaEquipo);
    }

    [Fact]
    public async Task ActualizarAsync_ConIdValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        var ligaActualizada = new Liga { Nombre = "Liga 1 Actualizada", CompeticionId = competicion.Id };

        // Act
        var resultado = await repository.ActualizarAsync(liga.Id, ligaActualizada);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 1 Actualizada", resultado.Nombre);
        
        var ligaEnBd = await context.Ligas.FindAsync(liga.Id);
        Assert.Equal("Liga 1 Actualizada", ligaEnBd.Nombre);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var liga = new Liga { Nombre = "Liga 1", CompeticionId = competicion.Id };
        await context.Ligas.AddAsync(liga);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EliminarAsync(liga.Id);

        // Assert
        Assert.True(resultado);
        var ligaEnBd = await context.Ligas.FindAsync(liga.Id);
        Assert.Null(ligaEnBd);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new LigaRepository(context);

        // Act
        var resultado = await repository.EliminarAsync(999);

        // Assert
        Assert.False(resultado);
    }
}


