using Derby.Backend.Dtos;
using Derby.Backend.Services;
using Derby.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Derby.Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Derby.Backend.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ArbitroController : ControllerBase
{
    private readonly IArbitroService _service;
    private readonly IEventoPartidoService _eventoService;
    private readonly DerbyContext _context;
    private readonly ILogger<ArbitroController> _logger;

    public ArbitroController(IArbitroService service, IEventoPartidoService eventoService, DerbyContext context, ILogger<ArbitroController> logger)
    {
        _service = service;
        _eventoService = eventoService;
        _context = context;
        _logger = logger;
    }

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

    [HttpGet("{arbitroId}/partidos")]
    public async Task<ActionResult<List<Partido>>> ObtenerMisPartidos(int arbitroId)
    {
        var partidos = await _context.Partidos
            .Where(p => p.ArbitroId == arbitroId)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .ToListAsync();
        return Ok(partidos);
    }

    [HttpGet("{arbitroId}/partidos/pendientes")]
    public async Task<ActionResult<List<Partido>>> ObtenerPartidosPendientes(int arbitroId)
    {
        var partidos = await _context.Partidos
            .Where(p => p.ArbitroId == arbitroId && p.Estado != "Finalizado")
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .ToListAsync();
        return Ok(partidos);
    }

    [HttpGet("{arbitroId}/historial")]
    public async Task<ActionResult<List<Partido>>> ObtenerHistorialPartidos(int arbitroId)
    {
        var partidos = await _context.Partidos
            .Where(p => p.ArbitroId == arbitroId && p.Estado == "Finalizado")
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .OrderByDescending(p => p.FechaHora)
            .ToListAsync();
        return Ok(partidos);
    }

    [HttpPost("actas")]
    public async Task<IActionResult> CrearActa([FromBody] Partido partido)
    {
        var p = await _context.Partidos.FindAsync(partido.Id);
        if (p == null)
            return NotFound(new { error = "Partido no encontrado" });

        p.GolesLocal = partido.GolesLocal;
        p.GolesVisitante = partido.GolesVisitante;
        p.Estado = "Finalizado";

        _context.Partidos.Update(p);
        await _context.SaveChangesAsync();
        return Ok(p);
    }

    [AllowAnonymous]
    [HttpGet("partidos/{partidoId}/eventos")]
    public async Task<IActionResult> ObtenerEventos(int partidoId)
    {
        var eventos = await _eventoService.ObtenerEventosAsync(partidoId);
        return Ok(eventos);
    }

    [HttpPost("partidos/{partidoId}/eventos")]
    public async Task<IActionResult> AñadirEvento(int partidoId, [FromBody] EventoPartidoRequestDto dto)
    {
        var evento = await _eventoService.AñadirEventoAsync(partidoId, dto);
        if (evento == null)
            return BadRequest(new { error = "TipoEvento no válido. Usa: Gol, TarjetaAmarilla o TarjetaRoja" });
        return Ok(evento);
    }

    [HttpDelete("partidos/{partidoId}/eventos/{eventoId}")]
    public async Task<IActionResult> EliminarEvento(int partidoId, int eventoId)
    {
        var eliminado = await _eventoService.EliminarEventoAsync(eventoId);
        return eliminado ? Ok() : NotFound(new { error = "Evento no encontrado" });
    }

    // Cerrar acta
    [HttpPost("partidos/{partidoId}/cerrar")]
    public async Task<IActionResult> CerrarActa(int partidoId)
    {
        var partido = await _eventoService.CerrarActaAsync(partidoId);
        if (partido == null)
            return NotFound(new { error = "Partido no encontrado" });
        return Ok(partido);
    }
}