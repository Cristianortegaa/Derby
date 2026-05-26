using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Xunit;

namespace Derby.Tests.Mappers;

public class UsuarioMapperTests
{
    [Fact]
    public void ToDto_DeberiaMapearEmailRolYToken()
    {
        // ==================== ARRANGE ====================
        var usuario = new Usuario { Id = 3, Email = "user@derby.com", Rol = Rol.Administrador, Contraseña = "hash" };

        // ==================== ACT ====================
        var dto = usuario.ToDto("mi-token-123");

        // ==================== ASSERT ====================
        Assert.Equal(3, dto.Id);
        Assert.Equal("user@derby.com", dto.Email);
        Assert.Equal("Administrador", dto.Rol);
        Assert.Equal("mi-token-123", dto.Token);
    }

    [Fact]
    public void ToDto_SinArbitroAsociado_ArbitroIdYNombreDeberianSerNull()
    {
        // ==================== ARRANGE ====================
        var usuario = new Usuario { Id = 1, Email = "x@x.com", Rol = Rol.Aficionado, Contraseña = "", ArbitroId = null, Arbitro = null };

        // ==================== ACT ====================
        var dto = usuario.ToDto("tok");

        // ==================== ASSERT ====================
        Assert.Null(dto.ArbitroId);
        Assert.Null(dto.NombreArbitro);
    }

    [Fact]
    public void ToDto_ConArbitroAsociado_DeberiaMapearNombreCompleto()
    {
        // ==================== ARRANGE ====================
        var arbitro = new Arbitro { Id = 5, Nombre = "Pedro", Apellidos = "Ruiz", NumeroColegiado = "" };
        var usuario = new Usuario { Id = 9, Email = "arb@derby.com", Rol = Rol.Arbitro, Contraseña = "h", ArbitroId = 5, Arbitro = arbitro };

        // ==================== ACT ====================
        var dto = usuario.ToDto("tok");

        // ==================== ASSERT ====================
        Assert.Equal(5, dto.ArbitroId);
        Assert.Equal("Pedro Ruiz", dto.NombreArbitro);
    }

    [Fact]
    public void ToDto_RolAficionado_DeberiaMapearRolComoString()
    {
        // ==================== ARRANGE ====================
        var usuario = new Usuario { Id = 1, Email = "fan@derby.com", Rol = Rol.Aficionado, Contraseña = "" };

        // ==================== ACT ====================
        var dto = usuario.ToDto("tok");

        // ==================== ASSERT ====================
        Assert.Equal("Aficionado", dto.Rol);
    }

    [Fact]
    public void ToDto_RolArbitro_DeberiaMapearRolComoString()
    {
        // ==================== ARRANGE ====================
        var usuario = new Usuario { Id = 2, Email = "arb@derby.com", Rol = Rol.Arbitro, Contraseña = "" };

        // ==================== ACT ====================
        var dto = usuario.ToDto("tok");

        // ==================== ASSERT ====================
        Assert.Equal("Arbitro", dto.Rol);
    }
}
