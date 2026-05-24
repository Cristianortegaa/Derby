using CSharpFunctionalExtensions;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;

namespace Derby.Backend.Services;

public interface IUsuarioService
{
    Task<Result<UsuarioResponseDto, DerbyError>> RegistrarAsync(RegistroRequestDto dto);
    Task<Result<UsuarioResponseDto, DerbyError>> LoginAsync(UsuarioRequestDto dto);
    Task<Result<List<UsuarioResponseDto>, DerbyError>> ObtenerTodosAsync();
    Task<Result<UsuarioResponseDto, DerbyError>> ObtenerPorIdAsync(int id);
    Task<Result<UsuarioResponseDto, DerbyError>> ActualizarAsync(int id, UsuarioRequestDto dto);
    Task<Result<bool, DerbyError>> EliminarAsync(int id);
}
