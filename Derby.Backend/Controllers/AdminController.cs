using Derby.Backend.Data;
using Derby.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derby.Backend.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly DerbyContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(DerbyContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
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
        lig.Equipos = liga.Equipos;
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
    public async Task<ActionResult<List<Equipo>>> ObtenerEquipos()
    {
        var equipos = await _context.Equipos.ToListAsync();
        return Ok(equipos);
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
        eq.Division = equipo.Division;

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
}