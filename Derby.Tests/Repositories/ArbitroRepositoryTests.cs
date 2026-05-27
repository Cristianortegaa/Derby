using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Derby.Tests.Repositories;

public class ArbitroRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task CrearAsync_ConArbitroValido_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);
        var arbitro = new Arbitro { Nombre = "Juan", Apellidos = "Pérez", NumeroColegiado = "A001" };

        // Act
        var resultado = await repository.CrearAsync(arbitro);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Juan", resultado.Nombre);
        var arbitroEnBd = await context.Arbitros.FirstOrDefaultAsync(a => a.Nombre == "Juan");
        Assert.NotNull(arbitroEnBd);
    }

    [Fact]
    public async Task ObtenerTodosAsync_RetornaListaDeArbitros()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);
        
        var arbitro1 = new Arbitro { Nombre = "Juan", Apellidos = "Pérez", NumeroColegiado = "A001" };
        var arbitro2 = new Arbitro { Nombre = "Carlos", Apellidos = "García", NumeroColegiado = "A002" };
        
        await context.Arbitros.AddAsync(arbitro1);
        await context.Arbitros.AddAsync(arbitro2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaArbitro()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);
        
        var arbitro = new Arbitro { Nombre = "Juan", Apellidos = "Pérez", NumeroColegiado = "A001" };
        await context.Arbitros.AddAsync(arbitro);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(arbitro.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Juan", resultado.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ActualizarAsync_ConArbitroValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);
        
        var arbitro = new Arbitro { Nombre = "Juan", Apellidos = "Pérez", NumeroColegiado = "A001" };
        await context.Arbitros.AddAsync(arbitro);
        await context.SaveChangesAsync();

        arbitro.Nombre = "Carlos";

        // Act
        var resultado = await repository.ActualizarAsync(arbitro);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Carlos", resultado.Nombre);
        
        var arbitroActualizado = await context.Arbitros.FindAsync(arbitro.Id);
        Assert.Equal("Carlos", arbitroActualizado.Nombre);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);
        
        var arbitro = new Arbitro { Nombre = "Juan", Apellidos = "Pérez", NumeroColegiado = "A001" };
        await context.Arbitros.AddAsync(arbitro);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EliminarAsync(arbitro.Id);

        // Assert
        Assert.True(resultado);
        var arbitroEnBd = await context.Arbitros.FindAsync(arbitro.Id);
        Assert.Null(arbitroEnBd);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var logger = Mock.Of<ILogger<ArbitroRepository>>();
        var repository = new ArbitroRepository(context, logger);

        // Act
        var resultado = await repository.EliminarAsync(999);

        // Assert
        Assert.False(resultado);
    }
}







