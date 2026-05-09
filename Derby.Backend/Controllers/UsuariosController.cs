﻿using Derby.Backend.Dtos;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Derby.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuarioService service, ILogger<UsuariosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<UsuarioResponseDto>> Registro([FromBody] RegistroRequestDto dto)
    {
        _logger.LogInformation("Solicitud de registro para: {Email}", dto.Email);
        var result = await _service.RegistrarAsync(dto);

        if (result.IsFailure)
        {
            _logger.LogWarning("Registro fallido: {Error}", result.Error.Message);
            return BadRequest(new { error = result.Error.Message });
        }

        return Created($"api/usuarios/{result.Value.Id}", result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioResponseDto>> Login([FromBody] UsuarioRequestDto dto)
    {
        try
        {
            _logger.LogInformation("Solicitud de login para: {Email}", dto.Email);
            var result = await _service.LoginAsync(dto);

            if (result.IsFailure)
            {
                _logger.LogWarning("Login fallido: {Error}", result.Error.Message);
                return Unauthorized(new { error = result.Error.Message });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login para: {Email}", dto.Email);
            return StatusCode(500, new { error = "Error en el servidor", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponseDto>>> ObtenerTodos()
    {
        var result = await _service.ObtenerTodosAsync();

        if (result.IsFailure)
        {
            _logger.LogWarning("Error al obtener usuarios: {Error}", result.Error.Message);
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioResponseDto>> ObtenerPorId(int id)
    {
        var result = await _service.ObtenerPorIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogWarning("Usuario no encontrado: {Id}", id);
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UsuarioResponseDto>> Actualizar(int id, [FromBody] UsuarioRequestDto dto)
    {
        _logger.LogInformation("Actualizando usuario: {Id}", id);
        var result = await _service.ActualizarAsync(id, dto);

        if (result.IsFailure)
        {
            _logger.LogWarning("Error al actualizar usuario: {Error}", result.Error.Message);
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        _logger.LogInformation("Eliminando usuario: {Id}", id);
        var result = await _service.EliminarAsync(id);

        if (result.IsFailure)
        {
            _logger.LogWarning("Error al eliminar usuario: {Error}", result.Error.Message);
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }
}
