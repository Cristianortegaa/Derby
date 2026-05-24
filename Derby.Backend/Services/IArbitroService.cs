using CSharpFunctionalExtensions;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;

namespace Derby.Backend.Services;

public interface IArbitroService
{
    Task<Result<IEnumerable<ArbitroResponseDto>, DerbyError>> ObtenerTodosAsync();
    Task<Result<ArbitroResponseDto, DerbyError>> ObtenerPorIdAsync(int id);
    Task<Result<ArbitroResponseDto, DerbyError>> CrearAsync(ArbitroRequestDto dto);
    Task<Result<ArbitroResponseDto, DerbyError>> ActualizarAsync(int id, ArbitroRequestDto dto);
    Task<Result<bool, DerbyError>> EliminarAsync(int id);
}
