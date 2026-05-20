using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging; 
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class EquipoService : IEquipoService
{
    private readonly IEquipoRepository _repository;
    private readonly ILogger<EquipoService> _logger; 

    public EquipoService(IEquipoRepository repository, ILogger<EquipoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<EquipoResponseDto>, DerbyError>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Servicio: Solicitando la lista completa de equipos.");
        var equipos = await _repository.ObtenerTodosConLigaAsync();
        var dtos = equipos.Select(e => e.ToDto());
        _logger.LogInformation("Servicio: Se han obtenido {Cantidad} equipos correctamente.", equipos.Count());
        return Result.Success<IEnumerable<EquipoResponseDto>, DerbyError>(dtos);
    }

    public async Task<Result<EquipoResponseDto, DerbyError>> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Servicio: Buscando el equipo con ID {Id}.", id);
        var equipo = await _repository.ObtenerPorIdAsync(id);

        if (equipo == null)
        {
            _logger.LogInformation("Servicio: No se encontró el equipo con ID {Id}.", id);
            return Result.Failure<EquipoResponseDto, DerbyError>(
                new NotFoundError($"El equipo con ID {id} no existe."));
        }

        _logger.LogInformation("Servicio: Equipo con ID {Id} encontrado con éxito.", id);
        return Result.Success<EquipoResponseDto, DerbyError>(equipo.ToDto());
    }

    public async Task<Result<EquipoResponseDto, DerbyError>> CrearAsync(EquipoRequestDto dto)
    {
        _logger.LogInformation("Servicio: Iniciando la creación del equipo '{Nombre}'.", dto.Nombre);

        var nuevoEquipo = dto.ToEntity();
        var equipoCreado = await _repository.CrearAsync(nuevoEquipo);
        
        _logger.LogInformation("Servicio: Equipo '{Nombre}' creado exitosamente con el ID {Id}.", equipoCreado.Nombre, equipoCreado.Id);
        return Result.Success<EquipoResponseDto, DerbyError>(equipoCreado.ToDto());
    }

    public async Task<Result<EquipoResponseDto, DerbyError>> ActualizarAsync(int id, EquipoRequestDto dto)
    {
        _logger.LogInformation("Servicio: Intentando actualizar el equipo con ID {Id}.", id);
        var equipoExistente = await _repository.ObtenerPorIdAsync(id);
        
        if (equipoExistente == null) 
        {
            _logger.LogInformation("Servicio: Fallo al actualizar. El equipo con ID {Id} no existe.", id);
            return Result.Failure<EquipoResponseDto, DerbyError>(
                new NotFoundError($"No se puede actualizar. El equipo con ID {id} no existe."));
        }

        equipoExistente.Nombre = dto.Nombre;
        equipoExistente.EscudoUrl = dto.EscudoUrl;
        equipoExistente.Sede = dto.Sede;
        equipoExistente.Entrenador = dto.Entrenador;

        var equipoActualizado = await _repository.ActualizarAsync(equipoExistente);
        
        _logger.LogInformation("Servicio: Equipo con ID {Id} actualizado correctamente.", id);
        return Result.Success<EquipoResponseDto, DerbyError>(equipoActualizado.ToDto());
    }

    public async Task<Result<bool, DerbyError>> EliminarAsync(int id)
    {
        _logger.LogInformation("Servicio: Intentando eliminar el equipo con ID {Id}.", id);
        var eliminado = await _repository.EliminarAsync(id);
        
        if (!eliminado)
        {
            _logger.LogInformation("Servicio: Fallo al eliminar. El equipo con ID {Id} no fue encontrado.", id);
            return Result.Failure<bool, DerbyError>(
                new NotFoundError($"No se puede borrar. El equipo con ID {id} no existe."));
        }

        _logger.LogInformation("Servicio: Equipo con ID {Id} eliminado exitosamente.", id);
        return Result.Success<bool, DerbyError>(true);
    }
}