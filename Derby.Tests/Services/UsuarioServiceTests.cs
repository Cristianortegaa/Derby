using System.Security.Cryptography;
using System.Text;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository>      _mockUsuarioRepo = new();
    private readonly Mock<IArbitroRepository>      _mockArbitroRepo = new();
    private readonly Mock<ILogger<UsuarioService>> _mockLogger      = new();
    private readonly Mock<IConfiguration>          _mockConfig      = new();
    private readonly UsuarioService                _servicio;

    public UsuarioServiceTests()
    {
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("derby-clave-secreta-super-larga-2026-tfg-cristian");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("derby-backend");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("derby-frontend");

        _servicio = new UsuarioService(_mockUsuarioRepo.Object, _mockArbitroRepo.Object, _mockLogger.Object, _mockConfig.Object);
    }

    // Helper: genera el mismo hash SHA256 que usa el servicio
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    // =========================================================================
    // Login
    // =========================================================================

    [Fact]
    public async Task LoginAsync_CuandoLasCredencialesSonCorrectas_DeberiaRetornarUsuarioConToken()
    {
        // ==================== ARRANGE ====================
        string emailTest     = "admin@derby.com";
        string contrasenaTest = "Password1";

        var usuarioEnBD = new Usuario
        {
            Id         = 1,
            Email      = emailTest,
            Contraseña = HashPassword(contrasenaTest),
            Rol        = Rol.Administrador
        };
        _mockUsuarioRepo
            .Setup(r => r.ObtenerPorEmailAsync(emailTest))
            .ReturnsAsync(usuarioEnBD);

        var dto = new UsuarioRequestDto { Email = emailTest, Contrasena = contrasenaTest };

        // ==================== ACT ====================
        var resultado = await _servicio.LoginAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(emailTest, resultado.Value.Email);
        Assert.NotNull(resultado.Value.Token);
        Assert.NotEmpty(resultado.Value.Token);
        _mockUsuarioRepo.Verify(r => r.ObtenerPorEmailAsync(emailTest), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_CuandoElEmailNoExiste_DeberiaRetornarUnauthorizedErrorYNOBuscarMas()
    {
        // ==================== ARRANGE ====================
        string emailInexistente = "noexiste@derby.com";
        _mockUsuarioRepo
            .Setup(r => r.ObtenerPorEmailAsync(emailInexistente))
            .ReturnsAsync((Usuario?)null);

        var dto = new UsuarioRequestDto { Email = emailInexistente, Contrasena = "cualquier" };

        // ==================== ACT ====================
        var resultado = await _servicio.LoginAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<UnauthorizedError>(resultado.Error);
        Assert.Equal("Credenciales inválidas", resultado.Error.Message);

        // Nunca debe intentar crear ni actualizar nada
        _mockUsuarioRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CuandoLaContrasenaEsIncorrecta_DeberiaRetornarUnauthorizedError()
    {
        // ==================== ARRANGE ====================
        string emailTest = "admin@derby.com";
        var usuarioEnBD  = new Usuario
        {
            Id         = 1,
            Email      = emailTest,
            Contraseña = HashPassword("contrasenaCorrecta"),
            Rol        = Rol.Administrador
        };
        _mockUsuarioRepo.Setup(r => r.ObtenerPorEmailAsync(emailTest)).ReturnsAsync(usuarioEnBD);

        var dto = new UsuarioRequestDto { Email = emailTest, Contrasena = "contrasenaINCORRECTA" };

        // ==================== ACT ====================
        var resultado = await _servicio.LoginAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<UnauthorizedError>(resultado.Error);
        Assert.Equal("Credenciales inválidas", resultado.Error.Message);
        _mockUsuarioRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Never);
    }

    // =========================================================================
    // Registrar
    // =========================================================================

    [Fact]
    public async Task RegistrarAsync_CuandoElEmailEsNuevo_DeberiaCrearElUsuarioYRetornarlo()
    {
        // ==================== ARRANGE ====================
        string emailNuevo = "nuevo@derby.com";
        var dto = new RegistroRequestDto
        {
            Email      = emailNuevo,
            Contrasena = "Pass1",
            Rol        = "aficionado",
            Nombre     = "",
            Apellidos  = ""
        };
        var usuarioCreado = new Usuario { Id = 5, Email = emailNuevo, Contraseña = HashPassword("Pass1"), Rol = Rol.Aficionado };

        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync(emailNuevo)).ReturnsAsync(false);
        _mockUsuarioRepo.Setup(r => r.CrearAsync(It.IsAny<Usuario>())).ReturnsAsync(usuarioCreado);

        // ==================== ACT ====================
        var resultado = await _servicio.RegistrarAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(emailNuevo, resultado.Value.Email);
        _mockUsuarioRepo.Verify(r => r.EmailExisteAsync(emailNuevo), Times.Once);
        _mockUsuarioRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_CuandoElEmailYaEstaRegistrado_DeberiaRetornarBadRequestErrorYNOCrearNada()
    {
        // ==================== ARRANGE ====================
        string emailDuplicado = "existente@derby.com";
        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync(emailDuplicado)).ReturnsAsync(true);

        var dto = new RegistroRequestDto { Email = emailDuplicado, Contrasena = "pass", Rol = "aficionado", Nombre = "", Apellidos = "" };

        // ==================== ACT ====================
        var resultado = await _servicio.RegistrarAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);
        Assert.Equal("Email ya registrado", resultado.Error.Message);

        // Si el email ya existe, nunca debe intentar crear el usuario
        _mockUsuarioRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Never);
        _mockArbitroRepo.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarAsync_CuandoElRolEsArbitro_TambienDeberiaCrearElArbitroAsociado()
    {
        // ==================== ARRANGE ====================
        string emailArbitro = "arbitro@derby.com";
        var dto = new RegistroRequestDto
        {
            Email      = emailArbitro,
            Contrasena = "pass",
            Rol        = "arbitro",
            Nombre     = "Pedro",
            Apellidos  = "Sánchez"
        };
        var arbitroCreado = new Arbitro { Id = 3, Nombre = "Pedro", Apellidos = "Sánchez", NumeroColegiado = "" };
        var usuarioCreado = new Usuario { Id = 7, Email = emailArbitro, Rol = Rol.Arbitro, ArbitroId = 3, Contraseña = "" };

        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync(emailArbitro)).ReturnsAsync(false);
        _mockArbitroRepo.Setup(r => r.CrearAsync(It.IsAny<Arbitro>())).ReturnsAsync(arbitroCreado);
        _mockUsuarioRepo.Setup(r => r.CrearAsync(It.IsAny<Usuario>())).ReturnsAsync(usuarioCreado);

        // ==================== ACT ====================
        var resultado = await _servicio.RegistrarAsync(dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);

        // Para un árbitro, el repositorio de árbitros DEBE ser llamado una vez
        _mockArbitroRepo.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Once);
        _mockUsuarioRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_CuandoElRolEsAficionado_NODeberiaCrearNingunArbitro()
    {
        // ==================== ARRANGE ====================
        var dto = new RegistroRequestDto { Email = "fan@derby.com", Contrasena = "pass", Rol = "aficionado", Nombre = "Ana", Apellidos = "Ruiz" };
        var usuarioCreado = new Usuario { Id = 8, Email = "fan@derby.com", Rol = Rol.Aficionado, Contraseña = "" };

        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync("fan@derby.com")).ReturnsAsync(false);
        _mockUsuarioRepo.Setup(r => r.CrearAsync(It.IsAny<Usuario>())).ReturnsAsync(usuarioCreado);

        // ==================== ACT ====================
        await _servicio.RegistrarAsync(dto);

        // ==================== ASSERT ====================
        // Un aficionado no tiene árbitro asociado — el repo de árbitros NUNCA debe ser llamado
        _mockArbitroRepo.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarAsync_CuandoElRolEsAdministrador_NODeberiaCrearNingunArbitro()
    {
        // ==================== ARRANGE ====================
        var dto = new RegistroRequestDto { Email = "adm@derby.com", Contrasena = "pass", Rol = "administrador", Nombre = "", Apellidos = "" };
        var usuarioCreado = new Usuario { Id = 9, Email = "adm@derby.com", Rol = Rol.Administrador, Contraseña = "" };

        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync("adm@derby.com")).ReturnsAsync(false);
        _mockUsuarioRepo.Setup(r => r.CrearAsync(It.IsAny<Usuario>())).ReturnsAsync(usuarioCreado);

        // ==================== ACT ====================
        await _servicio.RegistrarAsync(dto);

        // ==================== ASSERT ====================
        _mockArbitroRepo.Verify(r => r.CrearAsync(It.IsAny<Arbitro>()), Times.Never);
    }

    // =========================================================================
    // ObtenerTodos / ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodosLosUsuarios()
    {
        // ==================== ARRANGE ====================
        var usuariosEnBD = new List<Usuario>
        {
            new() { Id = 1, Email = "a@derby.com", Rol = Rol.Administrador, Contraseña = "" },
            new() { Id = 2, Email = "b@derby.com", Rol = Rol.Aficionado,    Contraseña = "" },
        };
        _mockUsuarioRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(usuariosEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerTodosAsync();

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal(2, resultado.Value.Count);
        _mockUsuarioRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElUsuarioExiste_DeberiaRetornarlo()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var usuarioEnBD = new Usuario { Id = idTest, Email = "a@derby.com", Rol = Rol.Administrador, Contraseña = "" };
        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(usuarioEnBD);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal("a@derby.com", resultado.Value.Email);
        _mockUsuarioRepo.Verify(r => r.ObtenerPorIdAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoElUsuarioNoExiste_DeberiaRetornarNotFoundError()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Usuario?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.ObtenerPorIdAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task ActualizarAsync_CuandoElUsuarioExisteYLosDatosSonValidos_DeberiaActualizarYRetornar()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var usuarioEnBD = new Usuario { Id = idTest, Email = "original@derby.com", Rol = Rol.Aficionado, Contraseña = HashPassword("pass") };
        var usuarioActualizado = new Usuario { Id = idTest, Email = "nuevo@derby.com", Rol = Rol.Aficionado, Contraseña = HashPassword("pass") };
        var dto = new UsuarioRequestDto { Email = "nuevo@derby.com", Contrasena = "", Rol = "aficionado" };

        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(usuarioEnBD);
        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync("nuevo@derby.com")).ReturnsAsync(false);
        _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>())).ReturnsAsync(usuarioActualizado);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.Equal("nuevo@derby.com", resultado.Value.Email);
        _mockUsuarioRepo.Verify(r => r.ActualizarAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElUsuarioNoExiste_DeberiaRetornarNotFoundError()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Usuario?)null);
        var dto = new UsuarioRequestDto { Email = "x@derby.com", Contrasena = "", Rol = "aficionado" };

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idInexistente, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        _mockUsuarioRepo.Verify(r => r.ActualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoElEmailNuevoYaEstaEnUso_DeberiaRetornarBadRequestError()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var usuarioEnBD = new Usuario { Id = idTest, Email = "original@derby.com", Rol = Rol.Aficionado, Contraseña = "" };
        var dto = new UsuarioRequestDto { Email = "duplicado@derby.com", Contrasena = "", Rol = "aficionado" };

        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(usuarioEnBD);
        _mockUsuarioRepo.Setup(r => r.EmailExisteAsync("duplicado@derby.com")).ReturnsAsync(true);

        // ==================== ACT ====================
        var resultado = await _servicio.ActualizarAsync(idTest, dto);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<BadRequestError>(resultado.Error);
        _mockUsuarioRepo.Verify(r => r.ActualizarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task EliminarAsync_CuandoElUsuarioExiste_DeberiaEliminarloyRetornarTrue()
    {
        // ==================== ARRANGE ====================
        int idTest = 1;
        var usuarioEnBD = new Usuario { Id = idTest, Email = "a@derby.com", Rol = Rol.Aficionado, Contraseña = "" };
        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idTest)).ReturnsAsync(usuarioEnBD);
        _mockUsuarioRepo.Setup(r => r.EliminarAsync(idTest)).Returns(Task.CompletedTask);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idTest);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsSuccess);
        Assert.True(resultado.Value);
        _mockUsuarioRepo.Verify(r => r.EliminarAsync(idTest), Times.Once);
    }

    [Fact]
    public async Task EliminarAsync_CuandoElUsuarioNoExiste_DeberiaRetornarNotFoundErrorYNOEliminarNada()
    {
        // ==================== ARRANGE ====================
        int idInexistente = 99;
        _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(idInexistente)).ReturnsAsync((Usuario?)null);

        // ==================== ACT ====================
        var resultado = await _servicio.EliminarAsync(idInexistente);

        // ==================== ASSERT ====================
        Assert.True(resultado.IsFailure);
        Assert.IsType<NotFoundError>(resultado.Error);
        _mockUsuarioRepo.Verify(r => r.EliminarAsync(It.IsAny<int>()), Times.Never);
    }
}
