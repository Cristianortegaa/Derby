using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Services;
using Derby.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArbitroController : ControllerBase
{
    private readonly IArbitroService _service;
    private readonly DerbyContext _context;
    private readonly ILogger<ArbitroController> _logger;

    public ArbitroController(IArbitroService service, DerbyContext context, ILogger<ArbitroController> logger)
    {
        _service = service;
        _context = context;
        _logger = logger;
    }

    // CRUD Básico para Gestión de Árbitros (Admin)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArbitroResponseDto>>> ObtenerTodos()
    {
        var resultado = await _service.ObtenerTodosAsync();
        return resultado.IsSuccess ? Ok(resultado.Value) : BadRequest(resultado.Error);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArbitroResponseDto>> ObtenerPorId(int id)
    {
        var resultado = await _service.ObtenerPorIdAsync(id);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound(resultado.Error);
    }

    [HttpPost]
    public async Task<ActionResult<ArbitroResponseDto>> Crear([FromBody] ArbitroRequestDto dto)
    {
        var resultado = await _service.CrearAsync(dto);
        return resultado.IsSuccess ? CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Value.Id }, resultado.Value) : BadRequest(resultado.Error);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ArbitroResponseDto>> Actualizar(int id, [FromBody] ArbitroRequestDto dto)
    {
        var resultado = await _service.ActualizarAsync(id, dto);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound(resultado.Error);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Eliminar(int id)
    {
        var resultado = await _service.EliminarAsync(id);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound(resultado.Error);
    }

    // Mis Partidos
    [HttpGet("{arbitroId}/partidos")]
    public async Task<ActionResult<List<Partido>>> ObtenerMisPartidos(int arbitroId)
    {
        var partidos = await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .ToListAsync();
        return Ok(partidos);
    }

    // Partidos Pendientes
    [HttpGet("{arbitroId}/partidos/pendientes")]
    public async Task<ActionResult<List<Partido>>> ObtenerPartidosPendientes(int arbitroId)
    {
        var partidos = await _context.Partidos
            .Where(p => Finalizado != true)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .ToListAsync();
        return Ok(partidos);
    }

    // Historial de Partidos
    [HttpGet("{arbitroId}/historial")]
    public async Task<ActionResult<List<Partido>>> ObtenerHistorialPartidos(int arbitroId)
    {
        var partidos = await _context.Partidos
            .Where(p => p.Finalizado == true)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
        return Ok(partidos);
    }

    // Crear Acta
    [HttpPost("actas")]
    public async Task<IActionResult> CrearActa([FromBody] Partido partido)
    {
        var p = await _context.Partidos.FindAsync(partido.Id);
        if (p == null)
            return NotFound(new { error = "Partido no encontrado" });

        p.GolesLocal = partido.GolesLocal;
        p.GolesVisitantes = partido.GolesVisitantes;
        p.Finalizado = true;
        
        _context.Partidos.Update(p);
        await _context.SaveChangesAsync();
        return Ok(p);
    }
}


