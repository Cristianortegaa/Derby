using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Tests.Repositories;

public class CompeticionRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task CrearAsync_ConCompeticionValida_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        var competicion = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };

        // Act
        var resultado = await repository.CrearAsync(competicion);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 2025", resultado.Nombre);
        var competicionEnBd = await context.Competiciones.FirstOrDefaultAsync(c => c.Nombre == "Liga 2025");
        Assert.NotNull(competicionEnBd);
    }

    [Fact]
    public async Task ObtenerTodasAsync_RetornaListaDeCompeticiones()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicion1 = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };
        var competicion2 = new Competicion { Nombre = "Copa 2025", Temporada = "2025/2026" };
        
        await context.Competiciones.AddRangeAsync(competicion1, competicion2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerTodasAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaCompeticion()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(competicion.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 2025", resultado.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerPorNombreYTemporadaAsync_ConValoresValidos_RetornaCompeticion()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorNombreYTemporadaAsync("Liga 2025", "2025/2026");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 2025", resultado.Nombre);
    }

    [Fact]
    public async Task FiltrarAsync_ConTemporada_RetornaSoloCompeticionesConEsaTemporada()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicion1 = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };
        var competicion2 = new Competicion { Nombre = "Liga 2024", Temporada = "2024/2025" };
        
        await context.Competiciones.AddRangeAsync(competicion1, competicion2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.FiltrarAsync(temporada: "2025/2026");

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal("Liga 2025", resultado.First().Nombre);
    }

    [Fact]
    public async Task ActualizarAsync_ConIdValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        var competicionActualizada = new Competicion { Nombre = "Liga 2025 Modificada", Temporada = "2025/2026" };

        // Act
        var resultado = await repository.ActualizarAsync(competicion.Id, competicionActualizada);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Liga 2025 Modificada", resultado.Nombre);
        
        var competicionEnBd = await context.Competiciones.FindAsync(competicion.Id);
        Assert.Equal("Liga 2025 Modificada", competicionEnBd.Nombre);
    }

    [Fact]
    public async Task ActualizarAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicionActualizada = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };

        // Act
        var resultado = await repository.ActualizarAsync(999, competicionActualizada);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);
        
        var competicion = new Competicion { Nombre = "Liga 2025", Temporada = "2025/2026" };
        await context.Competiciones.AddAsync(competicion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EliminarAsync(competicion.Id);

        // Assert
        Assert.True(resultado);
        var competicionEnBd = await context.Competiciones.FindAsync(competicion.Id);
        Assert.Null(competicionEnBd);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new CompeticionRepository(context);

        // Act
        var resultado = await repository.EliminarAsync(999);

        // Assert
        Assert.False(resultado);
    }
}


