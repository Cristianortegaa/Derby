using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;

namespace Derby.Backend.Services;

public class ArbitroService : IArbitroService
{
    private readonly IArbitroRepository _repository;
    private readonly ILogger<ArbitroService> _logger;

    public ArbitroService(IArbitroRepository repository, ILogger<ArbitroService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ArbitroResponseDto>, DerbyError>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Servicio: Solicitando la lista completa de árbitros.");

        var arbitros = await _repository.ObtenerTodosAsync();
        var dtos = arbitros.Select(a => a.ToDto());

        _logger.LogInformation("Servicio: Se han obtenido {Cantidad} árbitros correctamente.", arbitros.Count());
        return Result.Success<IEnumerable<ArbitroResponseDto>, DerbyError>(dtos);
    }

    public async Task<Result<ArbitroResponseDto, DerbyError>> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Servicio: Buscando el árbitro con ID {Id}.", id);
        var arbitro = await _repository.ObtenerPorIdAsync(id);

        if (arbitro == null)
        {
            _logger.LogInformation("Servicio: No se encontró el árbitro con ID {Id}.", id);
            return Result.Failure<ArbitroResponseDto, DerbyError>(
                new NotFoundError($"El árbitro con ID {id} no existe."));
        }

        _logger.LogInformation("Servicio: Árbitro con ID {Id} encontrado con éxito.", id);
        return Result.Success<ArbitroResponseDto, DerbyError>(arbitro.ToDto());
    }

    public async Task<Result<ArbitroResponseDto, DerbyError>> CrearAsync(ArbitroRequestDto dto)
    {
        _logger.LogInformation("Servicio: Iniciando la creación del árbitro '{Nombre} {Apellidos}'.", dto.Nombre, dto.Apellidos);

        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellidos))
        {
            _logger.LogInformation("Servicio: Fallo de validación al crear. Nombre o apellidos vacíos.");
            return Result.Failure<ArbitroResponseDto, DerbyError>(
                new BadRequestError("El nombre y apellidos son requeridos."));
        }

        var nuevoArbitro = dto.ToEntity();
        var arbitroCreado = await _repository.CrearAsync(nuevoArbitro);

        _logger.LogInformation("Servicio: Árbitro '{Nombre} {Apellidos}' creado exitosamente con el ID {Id}.", arbitroCreado.Nombre, arbitroCreado.Apellidos, arbitroCreado.Id);
        return Result.Success<ArbitroResponseDto, DerbyError>(arbitroCreado.ToDto());
    }

    public async Task<Result<ArbitroResponseDto, DerbyError>> ActualizarAsync(int id, ArbitroRequestDto dto)
    {
        _logger.LogInformation("Servicio: Intentando actualizar el árbitro con ID {Id}.", id);
        var arbitroExistente = await _repository.ObtenerPorIdAsync(id);

        if (arbitroExistente == null)
        {
            _logger.LogInformation("Servicio: Fallo al actualizar. El árbitro con ID {Id} no existe.", id);
            return Result.Failure<ArbitroResponseDto, DerbyError>(
                new NotFoundError($"No se puede actualizar. El árbitro con ID {id} no existe."));
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellidos))
        {
            _logger.LogInformation("Servicio: Fallo de validación al actualizar árbitro ID {Id}. Nombre o apellidos vacíos.", id);
            return Result.Failure<ArbitroResponseDto, DerbyError>(
                new BadRequestError("El nombre y apellidos son requeridos."));
        }

        arbitroExistente.Nombre = dto.Nombre;
        arbitroExistente.Apellidos = dto.Apellidos;
        arbitroExistente.NumeroColegiado = dto.NumeroColegiado;

        var arbitroActualizado = await _repository.ActualizarAsync(arbitroExistente);

        _logger.LogInformation("Servicio: Árbitro con ID {Id} actualizado correctamente.", id);
        return Result.Success<ArbitroResponseDto, DerbyError>(arbitroActualizado.ToDto());
    }

    public async Task<Result<bool, DerbyError>> EliminarAsync(int id)
    {
        _logger.LogInformation("Servicio: Intentando eliminar el árbitro con ID {Id}.", id);
        var eliminado = await _repository.EliminarAsync(id);

        if (!eliminado)
        {
            _logger.LogInformation("Servicio: Fallo al eliminar. El árbitro con ID {Id} no fue encontrado.", id);
            return Result.Failure<bool, DerbyError>(
                new NotFoundError($"No se puede borrar. El árbitro con ID {id} no existe."));
        }

        _logger.LogInformation("Servicio: Árbitro con ID {Id} eliminado exitosamente.", id);
        return Result.Success<bool, DerbyError>(true);
    }
}

