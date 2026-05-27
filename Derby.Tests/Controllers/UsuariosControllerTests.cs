using CSharpFunctionalExtensions;
using Derby.Backend.Controllers;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Derby.Tests.Controllers;

public class UsuariosControllerTests
{
    private readonly Mock<IUsuarioService>             _mockService = new();
    private readonly Mock<ILogger<UsuariosController>> _mockLogger  = new();
    private readonly UsuariosController                _controller;

    public UsuariosControllerTests()
    {
        _controller = new UsuariosController(_mockService.Object, _mockLogger.Object);
    }

    // =========================================================================
    // Registro
    // =========================================================================

    [Fact]
    public async Task Registro_CuandoEsValido_DeberiaRetornar201()
    {
        // ==================== ARRANGE ====================
        var dto      = new RegistroRequestDto { Email = "nuevo@derby.com", Contrasena = "Pass123", Nombre = "Test", Apellidos = "User" };
        var response = new UsuarioResponseDto { Id = 1, Email = dto.Email, Rol = "Aficionado" };
        _mockService.Setup(s => s.RegistrarAsync(dto))
                    .ReturnsAsync(Result.Success<UsuarioResponseDto, DerbyError>(response));

        // ==================== ACT ====================
        var actionResult = await _controller.Registro(dto);
        var result = actionResult.Result as CreatedResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(201, result!.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task Registro_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        var dto = new RegistroRequestDto { Email = "dup@derby.com", Contrasena = "Pass123", Nombre = "X", Apellidos = "Y" };
        _mockService.Setup(s => s.RegistrarAsync(dto))
                    .ReturnsAsync(Result.Failure<UsuarioResponseDto, DerbyError>(new BadRequestError("El email ya está en uso")));

        // ==================== ACT ====================
        var actionResult = await _controller.Registro(dto);
        var result = actionResult.Result as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // Login
    // =========================================================================

    [Fact]
    public async Task Login_CuandoCredencialesValidas_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto      = new UsuarioRequestDto { Email = "admin@derby.com", Contrasena = "Admin@123" };
        var response = new UsuarioResponseDto { Id = 1, Email = dto.Email, Rol = "Admin", Token = "jwt-token" };
        _mockService.Setup(s => s.LoginAsync(dto))
                    .ReturnsAsync(Result.Success<UsuarioResponseDto, DerbyError>(response));

        // ==================== ACT ====================
        var actionResult = await _controller.Login(dto);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task Login_CuandoCredencialesInvalidas_DeberiaRetornar401()
    {
        // ==================== ARRANGE ====================
        var dto = new UsuarioRequestDto { Email = "nadie@derby.com", Contrasena = "Mal" };
        _mockService.Setup(s => s.LoginAsync(dto))
                    .ReturnsAsync(Result.Failure<UsuarioResponseDto, DerbyError>(new UnauthorizedError("Credenciales inválidas")));

        // ==================== ACT ====================
        var actionResult = await _controller.Login(dto);
        var result = actionResult.Result as UnauthorizedObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(401, result!.StatusCode);
    }

    [Fact]
    public async Task Login_CuandoExcepcion_DeberiaRetornar500()
    {
        // ==================== ARRANGE ====================
        var dto = new UsuarioRequestDto { Email = "crash@derby.com", Contrasena = "X" };
        _mockService.Setup(s => s.LoginAsync(dto))
                    .ThrowsAsync(new Exception("Error de base de datos"));

        // ==================== ACT ====================
        var actionResult = await _controller.Login(dto);
        var result = actionResult.Result as ObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(500, result!.StatusCode);
    }

    // =========================================================================
    // ObtenerTodos
    // =========================================================================

    [Fact]
    public async Task ObtenerTodos_DeberiaRetornar200ConLista()
    {
        // ==================== ARRANGE ====================
        var lista = new List<UsuarioResponseDto>
        {
            new() { Id = 1, Email = "a@derby.com", Rol = "Admin" },
            new() { Id = 2, Email = "b@derby.com", Rol = "Aficionado" },
        };
        _mockService.Setup(s => s.ObtenerTodosAsync())
                    .ReturnsAsync(Result.Success<List<UsuarioResponseDto>, DerbyError>(lista));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerTodos();
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(lista, result.Value);
    }

    // =========================================================================
    // ObtenerPorId
    // =========================================================================

    [Fact]
    public async Task ObtenerPorId_CuandoExiste_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var response = new UsuarioResponseDto { Id = 1, Email = "a@derby.com", Rol = "Admin" };
        _mockService.Setup(s => s.ObtenerPorIdAsync(1))
                    .ReturnsAsync(Result.Success<UsuarioResponseDto, DerbyError>(response));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerPorId(1);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task ObtenerPorId_CuandoNoExiste_DeberiaRetornar404()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.ObtenerPorIdAsync(99))
                    .ReturnsAsync(Result.Failure<UsuarioResponseDto, DerbyError>(new NotFoundError("Usuario no encontrado")));

        // ==================== ACT ====================
        var actionResult = await _controller.ObtenerPorId(99);
        var result = actionResult.Result as NotFoundObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }

    // =========================================================================
    // Actualizar
    // =========================================================================

    [Fact]
    public async Task Actualizar_CuandoEsValido_DeberiaRetornar200()
    {
        // ==================== ARRANGE ====================
        var dto      = new UsuarioRequestDto { Email = "nuevo@derby.com", Contrasena = "Pass123" };
        var response = new UsuarioResponseDto { Id = 1, Email = dto.Email, Rol = "Aficionado" };
        _mockService.Setup(s => s.ActualizarAsync(1, dto))
                    .ReturnsAsync(Result.Success<UsuarioResponseDto, DerbyError>(response));

        // ==================== ACT ====================
        var actionResult = await _controller.Actualizar(1, dto);
        var result = actionResult.Result as OkObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
    }

    [Fact]
    public async Task Actualizar_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        var dto = new UsuarioRequestDto { Email = "dup@derby.com", Contrasena = "Pass123" };
        _mockService.Setup(s => s.ActualizarAsync(1, dto))
                    .ReturnsAsync(Result.Failure<UsuarioResponseDto, DerbyError>(new BadRequestError("Email duplicado")));

        // ==================== ACT ====================
        var actionResult = await _controller.Actualizar(1, dto);
        var result = actionResult.Result as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }

    // =========================================================================
    // Eliminar
    // =========================================================================

    [Fact]
    public async Task Eliminar_CuandoEsValido_DeberiaRetornar204()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.EliminarAsync(1))
                    .ReturnsAsync(Result.Success<bool, DerbyError>(true));

        // ==================== ACT ====================
        var result = await _controller.Eliminar(1) as NoContentResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(204, result!.StatusCode);
    }

    [Fact]
    public async Task Eliminar_CuandoFalla_DeberiaRetornar400()
    {
        // ==================== ARRANGE ====================
        _mockService.Setup(s => s.EliminarAsync(99))
                    .ReturnsAsync(Result.Failure<bool, DerbyError>(new NotFoundError("Usuario no encontrado")));

        // ==================== ACT ====================
        var result = await _controller.Eliminar(99) as BadRequestObjectResult;

        // ==================== ASSERT ====================
        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
    }
}
