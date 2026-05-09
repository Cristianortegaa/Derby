using Derby.Backend.Dtos;
using Derby.Backend.Errors;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 

namespace Derby.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        private readonly IEquipoService _service;
        private readonly ILogger<EquiposController> _logger; 

        public EquiposController(IEquipoService service, ILogger<EquiposController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoResponseDto>>> GetEquipos()
        {
            _logger.LogInformation("Ejecutando GetEquipos");
            var result = await _service.ObtenerTodosAsync();
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Falló GetEquipos: {MensajeError}", result.Error.Message);
                return BadRequest(new { error = result.Error.Message });
            }
            
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoResponseDto>> GetEquipoById(int id)
        {
            _logger.LogInformation("Ejecutando GetEquipoById para el ID: {Id}", id);
            var result = await _service.ObtenerPorIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogWarning("Falló GetEquipoById ({Id}): {MensajeError}", id, result.Error.Message);
                
                if (result.Error is NotFoundError)
                    return NotFound(new { error = result.Error.Message });
                
                return BadRequest(new { error = result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<EquipoResponseDto>> CreateEquipo([FromBody] EquipoRequestDto dto)
        {
            _logger.LogInformation("Ejecutando CreateEquipo para el equipo: {Nombre}", dto.Nombre);
            var result = await _service.CrearAsync(dto);

            if (result.IsFailure)
            {
                _logger.LogWarning("Falló CreateEquipo: {MensajeError}", result.Error.Message);
                
                if (result.Error is EquipoYaInscritoError)
                    return Conflict(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error.Message });
            }

            return CreatedAtAction(nameof(GetEquipoById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EquipoResponseDto>> UpdateEquipo(int id, [FromBody] EquipoRequestDto dto)
        {
            _logger.LogInformation("Ejecutando UpdateEquipo para el ID: {Id}", id);
            var result = await _service.ActualizarAsync(id, dto);

            if (result.IsFailure)
            {
                _logger.LogWarning("Falló UpdateEquipo ({Id}): {MensajeError}", id, result.Error.Message);
                
                if (result.Error is NotFoundError)
                    return NotFound(new { error = result.Error.Message });

                return BadRequest(new { error = result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEquipo(int id)
        {
            _logger.LogInformation("Ejecutando DeleteEquipo para el ID: {Id}", id);
            var result = await _service.EliminarAsync(id);

            if (result.IsFailure)
            {
                _logger.LogWarning("Falló DeleteEquipo ({Id}): {MensajeError}", id, result.Error.Message);
                
                if (result.Error is NotFoundError)
                    return NotFound(new { error = result.Error.Message });
                
                return BadRequest(new { error = result.Error.Message });
            }

            return NoContent();
        }
    }
}