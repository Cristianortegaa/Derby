using Derby.Backend.Dtos;
using Derby.Backend.Mappers;
using Derby.Backend.Models;
using Derby.Backend.Repositories;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Derby.Backend.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly ILigaService _ligaService;
    private readonly IJugadorService _jugadorService;
    private readonly IPartidoRepository _partidoRepository;
    private readonly ICompeticionService _competicionService;
    private readonly IArbitroService _arbitroService;
    private readonly IPartidoService _partidoService;
    private readonly IEquipoService _equipoService;

    public AdminController(ILogger<AdminController> logger, ILigaService ligaService, IJugadorService jugadorService, IPartidoRepository partidoRepository, ICompeticionService competicionService, IArbitroService arbitroService, IPartidoService partidoService, IEquipoService equipoService)
    {
        _logger = logger;
        _ligaService = ligaService;
        _jugadorService = jugadorService;
        _partidoRepository = partidoRepository;
        _competicionService = competicionService;
        _arbitroService = arbitroService;
        _partidoService = partidoService;
        _equipoService = equipoService;
    }

    // ─── Competiciones ────────────────────────────────────────────────────────

    [HttpGet("competiciones")]
    public async Task<ActionResult<List<CompeticionResponseDto>>> ObtenerCompeticiones()
    {
        var competiciones = await _competicionService.ObtenerTodasAsync();
        return Ok(competiciones);
    }

    [HttpPost("competiciones")]
    public async Task<ActionResult<CompeticionResponseDto>> CrearCompeticion([FromBody] Competicion competicion)
    {
        var creada = await _competicionService.CrearAsync(competicion);
        return Created($"api/admin/competiciones/{creada.Id}", creada);
    }

    [HttpPut("competiciones/{id}")]
    public async Task<IActionResult> ActualizarCompeticion(int id, [FromBody] Competicion competicion)
    {
        var actualizada = await _competicionService.ActualizarAsync(id, competicion);
        if (actualizada == null)
            return NotFound(new { error = "Competición no encontrada" });

        return Ok(actualizada);
    }

    [HttpDelete("competiciones/{id}")]
    public async Task<IActionResult> EliminarCompeticion(int id)
    {
        var eliminada = await _competicionService.EliminarAsync(id);
        if (!eliminada)
            return NotFound(new { error = "Competición no encontrada" });

        return NoContent();
    }

    // ─── Ligas ────────────────────────────────────────────────────────────────

    [HttpGet("ligas")]
    public async Task<ActionResult<List<LigaResponseDto>>> ObtenerLigas()
    {
        var ligas = await _ligaService.ObtenerTodasAsync();
        return Ok(ligas);
    }

    [HttpGet("ligas/{id}")]
    public async Task<ActionResult<LigaResponseDto>> ObtenerLiga(int id)
    {
        var liga = await _ligaService.ObtenerPorIdAsync(id);
        if (liga == null)
            return NotFound(new { error = "Liga no encontrada" });
        return Ok(liga);
    }

    [HttpPost("ligas")]
    public async Task<ActionResult<LigaResponseDto>> CrearLiga([FromBody] LigaRequestDto dto)
    {
        var creada = await _ligaService.CrearAsync(dto);
        return Created($"api/admin/ligas/{creada.Id}", creada);
    }

    [HttpPut("ligas/{id}")]
    public async Task<IActionResult> ActualizarLiga(int id, [FromBody] LigaRequestDto dto)
    {
        var actualizada = await _ligaService.ActualizarAsync(id, dto);
        if (actualizada == null)
            return NotFound(new { error = "Liga no encontrada" });
        return Ok(actualizada);
    }

    [HttpDelete("ligas/{id}")]
    public async Task<IActionResult> EliminarLiga(int id)
    {
        var eliminada = await _ligaService.EliminarAsync(id);
        if (!eliminada)
            return NotFound(new { error = "Liga no encontrada" });
        return NoContent();
    }

    // ─── Equipos ──────────────────────────────────────────────────────────────

    [HttpGet("equipos")]
    public async Task<ActionResult<IEnumerable<EquipoResponseDto>>> ObtenerEquipos()
    {
        var result = await _equipoService.ObtenerTodosAsync();
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpPost("equipos")]
    public async Task<ActionResult<EquipoResponseDto>> CrearEquipo([FromBody] EquipoRequestDto dto)
    {
        var result = await _equipoService.CrearAsync(dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return Created($"api/admin/equipos/{result.Value.Id}", result.Value);
    }

    [HttpPut("equipos/{id}")]
    public async Task<IActionResult> ActualizarEquipo(int id, [FromBody] EquipoRequestDto dto)
    {
        var result = await _equipoService.ActualizarAsync(id, dto);
        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpDelete("equipos/{id}")]
    public async Task<IActionResult> EliminarEquipo(int id)
    {
        var result = await _equipoService.EliminarAsync(id);
        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });
        return NoContent();
    }

    [HttpGet("equipos/sin-liga")]
    public async Task<IActionResult> ObtenerEquiposSinLiga()
    {
        var equipos = await _ligaService.ObtenerEquiposSinLigaAsync();
        return Ok(equipos);
    }

    // ─── Árbitros ─────────────────────────────────────────────────────────────

    [HttpGet("arbitros")]
    public async Task<ActionResult<IEnumerable<ArbitroResponseDto>>> ObtenerArbitros()
    {
        var result = await _arbitroService.ObtenerTodosAsync();
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpPost("arbitros")]
    public async Task<ActionResult<ArbitroResponseDto>> CrearArbitro([FromBody] ArbitroRequestDto dto)
    {
        var result = await _arbitroService.CrearAsync(dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return Created($"api/admin/arbitros/{result.Value.Id}", result.Value);
    }

    [HttpPut("arbitros/{id}")]
    public async Task<IActionResult> ActualizarArbitro(int id, [FromBody] ArbitroRequestDto dto)
    {
        var result = await _arbitroService.ActualizarAsync(id, dto);
        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpDelete("arbitros/{id}")]
    public async Task<IActionResult> EliminarArbitro(int id)
    {
        var result = await _arbitroService.EliminarAsync(id);
        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });
        return NoContent();
    }
    
    // ─── Partidos ─────────────────────────────────────────────────────────────

    [HttpGet("partidos")]
    public async Task<ActionResult<List<PartidoResponseDto>>> ObtenerPartidos()
    {
        var partidos = await _partidoService.ObtenerTodosAsync();
        return Ok(partidos);
    }

    [HttpPost("partidos")]
    public async Task<ActionResult<PartidoResponseDto>> CrearPartido([FromBody] PartidoRequestDto dto)
    {
        var creado = await _partidoService.CrearAsync(dto);
        return Created($"api/admin/partidos/{creado.Id}", creado);
    }

    [HttpPut("partidos/{id}")]
    public async Task<IActionResult> ActualizarPartido(int id, [FromBody] PartidoRequestDto dto)
    {
        var actualizado = await _partidoService.ActualizarAsync(id, dto);
        if (actualizado == null)
            return NotFound(new { error = "Partido no encontrado" });
        return Ok(actualizado);
    }

    [HttpDelete("partidos/{id}")]
    public async Task<IActionResult> EliminarPartido(int id)
    {
        var eliminado = await _partidoService.EliminarAsync(id);
        if (!eliminado)
            return NotFound(new { error = "Partido no encontrado" });
        return NoContent();
    }
    
    // ─── Equipos de una Liga ──────────────────────────────────────────────────

    [HttpGet("ligas/{id}/equipos")]
    public async Task<IActionResult> ObtenerEquiposLiga(int id)
    {
        var equipos = await _ligaService.ObtenerEquiposAsync(id);
        return Ok(equipos);
    }

    [HttpPost("ligas/{id}/equipos")]
    public async Task<IActionResult> AgregarEquipoLiga(int id, [FromBody] int equipoId)
    {
        try
        {
            await _ligaService.AgregarEquipoAsync(id, equipoId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("ligas/{id}/equipos/{equipoId}")]
    public async Task<IActionResult> QuitarEquipoLiga(int id, int equipoId)
    {
        await _ligaService.QuitarEquipoAsync(id, equipoId);
        return NoContent();
    }

    [HttpPost("ligas/{id}/generar-calendario")]
    public async Task<IActionResult> GenerarCalendario(int id)
    {
        try
        {
            var resultado = await _ligaService.GenerarCalendarioAsync(id);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpGet("equipos/{equipoId}/jugadores")]
    public async Task<IActionResult> ObtenerJugadores(int equipoId)
    {
        var jugadores = await _jugadorService.ObtenerPorEquipoAsync(equipoId);
        return Ok(jugadores);
    }

    [HttpPost("equipos/{equipoId}/jugadores")]
    public async Task<IActionResult> AgregarJugador(int equipoId, [FromBody] JugadorRequestDto dto)
    {
        try
        {
            await _jugadorService.AgregarAsync(equipoId, dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("jugadores/{id}")]
    public async Task<IActionResult> ActualizarJugador(int id, [FromBody] JugadorRequestDto dto)
    {
        try
        {
            await _jugadorService.ActualizarAsync(id, dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("jugadores/{id}")]
    public async Task<IActionResult> EliminarJugador(int id)
    {
        try
        {
            await _jugadorService.EliminarAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ─── Actas ────────────────────────────────────────────────────────────────

    [HttpGet("actas")]
    public async Task<IActionResult> ObtenerActas()
    {
        var partidos = await _partidoRepository.ObtenerFinalizadosAsync();
        return Ok(partidos);
    }

    [HttpPut("actas/{partidoId}")]
    public async Task<IActionResult> EditarActa(int partidoId, [FromBody] Partido datos)
    {
        var partido = await _partidoRepository.ActualizarGolesAsync(partidoId, datos.GolesLocal, datos.GolesVisitante);
        if (partido == null)
            return NotFound(new { error = "Partido no encontrado" });
        return Ok(partido);
    }
}