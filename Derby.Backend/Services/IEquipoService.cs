using CSharpFunctionalExtensions;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;

namespace Derby.Backend.Services;

public interface IEquipoService
{
    Task<Result<IEnumerable<EquipoResponseDto>, DerbyError>> ObtenerTodosAsync();
    
    Task<Result<EquipoResponseDto, DerbyError>> ObtenerPorIdAsync(int id);
    
    Task<Result<EquipoResponseDto, DerbyError>> CrearAsync(EquipoRequestDto dto);
    
    Task<Result<EquipoResponseDto, DerbyError>> ActualizarAsync(int id, EquipoRequestDto dto);
    
    Task<Result<bool, DerbyError>> EliminarAsync(int id);
}