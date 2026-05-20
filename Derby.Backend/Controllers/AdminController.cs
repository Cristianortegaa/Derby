using Derby.Backend.Data;
using Derby.Backend.Dtos;
using Derby.Backend.Models;
using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly DerbyContext _context;
    private readonly ILogger<AdminController> _logger;
    private readonly ILigaService _ligaService;
    private readonly IJugadorService _jugadorService;
    private readonly IEquipoService _equipoService;

    public AdminController(DerbyContext context, ILogger<AdminController> logger, ILigaService ligaService, IJugadorService jugadorService, IEquipoService equipoService)
    {
        _context = context;
        _logger = logger;
        _ligaService = ligaService;
        _jugadorService = jugadorService;
        _equipoService = equipoService;
    }

    // ─── Competiciones ────────────────────────────────────────────────────────

    [HttpGet("competiciones")]
    public async Task<ActionResult<List<Competicion>>> ObtenerCompeticiones()
    {
        var competiciones = await _context.Competiciones.ToListAsync();
        return Ok(competiciones);
    }

    [HttpPost("competiciones")]
    public async Task<ActionResult<Competicion>> CrearCompeticion([FromBody] Competicion competicion)
    {
        _context.Competiciones.Add(competicion);
        await _context.SaveChangesAsync();
        return Created($"api/admin/competiciones/{competicion.Id}", competicion);
    }

    [HttpPut("competiciones/{id}")]
    public async Task<IActionResult> ActualizarCompeticion(int id, [FromBody] Competicion competicion)
    {
        var comp = await _context.Competiciones.FindAsync(id);
        if (comp == null)
            return NotFound(new { error = "Competición no encontrada" });

        comp.Nombre = competicion.Nombre;
        comp.Temporada = competicion.Temporada;
        comp.Descripcion = competicion.Descripcion;
        comp.Estado = competicion.Estado;
        comp.TipoJuego = competicion.TipoJuego;
        comp.Grupo = competicion.Grupo;

        await _context.SaveChangesAsync();
        return Ok(comp);
    }

    [HttpDelete("competiciones/{id}")]
    public async Task<IActionResult> EliminarCompeticion(int id)
    {
        var comp = await _context.Competiciones.FindAsync(id);
        if (comp == null)
            return NotFound(new { error = "Competición no encontrada" });

        _context.Competiciones.Remove(comp);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ─── Ligas ────────────────────────────────────────────────────────────────

    [HttpGet("ligas")]
    public async Task<ActionResult<List<Liga>>> ObtenerLigas()
    {
        var ligas = await _context.Ligas.ToListAsync();
        return Ok(ligas);
    }

    [HttpGet("ligas/{id}")]
    public async Task<ActionResult<Liga>> ObtenerLiga(int id)
    {
        var liga = await _context.Ligas.FindAsync(id);
        if (liga == null)
            return NotFound(new { error = "Liga no encontrada" });
        return Ok(liga);
    }

    [HttpPost("ligas")]
    public async Task<ActionResult<Liga>> CrearLiga([FromBody] Liga liga)
    {
        _context.Ligas.Add(liga);
        await _context.SaveChangesAsync();
        return Created($"api/admin/ligas/{liga.Id}", liga);
    }

    [HttpPut("ligas/{id}")]
    public async Task<IActionResult> ActualizarLiga(int id, [FromBody] Liga liga)
    {
        var lig = await _context.Ligas.FindAsync(id);
        if (lig == null)
            return NotFound(new { error = "Liga no encontrada" });

        lig.Nombre = liga.Nombre;
        lig.CompeticionId = liga.CompeticionId;
        lig.Grupo = liga.Grupo;
        lig.Jornadas = liga.Jornadas;
        lig.JornadaActual = liga.JornadaActual;
        lig.Estado = liga.Estado;

        await _context.SaveChangesAsync();
        return Ok(lig);
    }

    [HttpDelete("ligas/{id}")]
    public async Task<IActionResult> EliminarLiga(int id)
    {
        var lig = await _context.Ligas.FindAsync(id);
        if (lig == null)
            return NotFound(new { error = "Liga no encontrada" });

        _context.Ligas.Remove(lig);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ─── Equipos ──────────────────────────────────────────────────────────────
    
    [HttpGet("equipos")]
    public async Task<IActionResult> ObtenerEquipos()
    {
        var result = await _equipoService.ObtenerTodosAsync();
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpPost("equipos")]
    public async Task<ActionResult<Equipo>> CrearEquipo([FromBody] Equipo equipo)
    {
        _context.Equipos.Add(equipo);
        await _context.SaveChangesAsync();
        return Created($"api/admin/equipos/{equipo.Id}", equipo);
    }

    [HttpPut("equipos/{id}")]
    public async Task<IActionResult> ActualizarEquipo(int id, [FromBody] Equipo equipo)
    {
        var eq = await _context.Equipos.FindAsync(id);
        if (eq == null)
            return NotFound(new { error = "Equipo no encontrado" });

        eq.Nombre = equipo.Nombre;
        eq.Sede = equipo.Sede;
        eq.Entrenador = equipo.Entrenador;
        eq.EscudoUrl = equipo.EscudoUrl;

        await _context.SaveChangesAsync();
        return Ok(eq);
    }

    [HttpDelete("equipos/{id}")]
    public async Task<IActionResult> EliminarEquipo(int id)
    {
        var eq = await _context.Equipos.FindAsync(id);
        if (eq == null)
            return NotFound(new { error = "Equipo no encontrado" });

        _context.Equipos.Remove(eq);
        await _context.SaveChangesAsync();
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
    public async Task<ActionResult<List<Arbitro>>> ObtenerArbitros()
    {
        var arbitros = await _context.Arbitros.ToListAsync();
        return Ok(arbitros);
    }

    [HttpPost("arbitros")]
    public async Task<ActionResult<Arbitro>> CrearArbitro([FromBody] Arbitro arbitro)
    {
        _context.Arbitros.Add(arbitro);
        await _context.SaveChangesAsync();
        return Created($"api/admin/arbitros/{arbitro.Id}", arbitro);
    }

    [HttpPut("arbitros/{id}")]
    public async Task<IActionResult> ActualizarArbitro(int id, [FromBody] Arbitro arbitro)
    {
        var arb = await _context.Arbitros.FindAsync(id);
        if (arb == null)
            return NotFound(new { error = "Árbitro no encontrado" });

        arb.Nombre = arbitro.Nombre;
        arb.Apellidos = arbitro.Apellidos;

        await _context.SaveChangesAsync();
        return Ok(arb);
    }

    [HttpDelete("arbitros/{id}")]
    public async Task<IActionResult> EliminarArbitro(int id)
    {
        var arb = await _context.Arbitros.FindAsync(id);
        if (arb == null)
            return NotFound(new { error = "Árbitro no encontrado" });

        _context.Arbitros.Remove(arb);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    // ─── Partidos ─────────────────────────────────────────────────────────────

    [HttpGet("partidos")]
    public async Task<ActionResult<List<Partido>>> ObtenerPartidos()
    {
        var partidos = await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Include(p => p.Liga)
            .Include(p => p.Arbitro)
            .ToListAsync();
        return Ok(partidos);
    }

    [HttpPost("partidos")]
    public async Task<ActionResult<Partido>> CrearPartido([FromBody] Partido partido)
    {
        _context.Partidos.Add(partido);
        await _context.SaveChangesAsync();
        return Created($"api/admin/partidos/{partido.Id}", partido);
    }

    [HttpPut("partidos/{id}")]
    public async Task<IActionResult> ActualizarPartido(int id, [FromBody] Partido partido)
    {
        var p = await _context.Partidos.FindAsync(id);
        if (p == null)
            return NotFound(new { error = "Partido no encontrado" });

        p.Jornada = partido.Jornada;
        p.LigaId = partido.LigaId;
        p.EquipoLocalId = partido.EquipoLocalId;
        p.EquipoVisitanteId = partido.EquipoVisitanteId;
        p.GolesLocal = partido.GolesLocal;
        p.GolesVisitante = partido.GolesVisitante;
        p.Estado = partido.Estado;
        p.FechaHora = partido.FechaHora;
        p.ArbitroId = partido.ArbitroId;

        await _context.SaveChangesAsync();
        return Ok(p);
    }

    [HttpDelete("partidos/{id}")]
    public async Task<IActionResult> EliminarPartido(int id)
    {
        var p = await _context.Partidos.FindAsync(id);
        if (p == null)
            return NotFound(new { error = "Partido no encontrado" });

        _context.Partidos.Remove(p);
        await _context.SaveChangesAsync();
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
}