using Xunit;
using Moq;
using Derby.Backend.Data;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Derby.Tests.Repositories;

public class UsuarioRepositoryTests
{
    private DerbyContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DerbyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DerbyContext(options);
    }

    [Fact]
    public async Task CrearAsync_ConUsuarioValido_GuardaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        var usuario = new Usuario { Email = "test@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };

        // Act
        var resultado = await repository.CrearAsync(usuario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("test@example.com", resultado.Email);
        var usuarioEnBd = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(usuarioEnBd);
    }

    [Fact]
    public async Task ObtenerTodosAsync_RetornaListaDeUsuarios()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        
        var usuario1 = new Usuario { Email = "test1@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };
        var usuario2 = new Usuario { Email = "test2@example.com", Contraseña = "pass123", Rol = Rol.Arbitro };
        
        await context.Usuarios.AddRangeAsync(usuario1, usuario2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerTodosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdValido_RetornaUsuario()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        
        var usuario = new Usuario { Email = "test@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorIdAsync(usuario.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("test@example.com", resultado.Email);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);

        // Act
        var resultado = await repository.ObtenerPorIdAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerPorEmailAsync_ConEmailValido_RetornaUsuario()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        
        var usuario = new Usuario { Email = "test@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ObtenerPorEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("test@example.com", resultado.Email);
    }

    [Fact]
    public async Task ObtenerPorEmailAsync_ConEmailInvalido_RetornaNulo()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);

        // Act
        var resultado = await repository.ObtenerPorEmailAsync("noexiste@example.com");

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ActualizarAsync_ConUsuarioValido_ActualizaEnLaBaseDatos()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        
        var usuario = new Usuario { Email = "test@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        usuario.Rol = Rol.Arbitro;

        // Act
        var resultado = await repository.ActualizarAsync(usuario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(Rol.Arbitro, resultado.Rol);
        
        var usuarioActualizado = await context.Usuarios.FindAsync(usuario.Id);
        Assert.Equal(Rol.Arbitro, usuarioActualizado.Rol);
    }

    [Fact]
    public async Task EliminarAsync_ConIdValido_EliminaDelaBD()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        
        var usuario = new Usuario { Email = "test@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        // Act
        await repository.EliminarAsync(usuario.Id);

        // Assert
        var usuarioEnBd = await context.Usuarios.FindAsync(usuario.Id);
        Assert.Null(usuarioEnBd);
    }

    [Fact]
    public async Task EmailExisteAsync_ConEmailValido_RetornaTrue()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);
        
        var usuario = new Usuario { Email = "test@example.com", Contraseña = "pass123", Rol = Rol.Aficionado };
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.EmailExisteAsync("test@example.com");

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public async Task EmailExisteAsync_ConEmailInvalido_RetornaFalse()
    {
        // Arrange
        using var context = CreateDbContext();
        var repository = new UsuarioRepository(context);

        // Act
        var resultado = await repository.EmailExisteAsync("noexiste@example.com");

        // Assert
        Assert.False(resultado);
    }
}





