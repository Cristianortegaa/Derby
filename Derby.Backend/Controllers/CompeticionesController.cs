using Derby.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Derby.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompeticionesController : ControllerBase
{
    private readonly ICompeticionService _competicionService;
    private readonly ILigaService _ligaService;
    private readonly ILogger<CompeticionesController> _logger;

    public CompeticionesController(ICompeticionService competicionService, ILigaService ligaService, ILogger<CompeticionesController> logger)
    {
        _competicionService = competicionService;
        _ligaService = ligaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las jornadas y partidos de una competición
    /// </summary>
    [HttpGet("{competicionId:int}/jornadas")]
    public async Task<IActionResult> ObtenerJornadas(int competicionId)
    {
        try
        {
            var jornadas = await _competicionService.ObtenerJornadasAsync(competicionId);
            return Ok(jornadas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener jornadas");
            return StatusCode(500, new { message = "Error al obtener jornadas" });
        }
    }

    /// <summary>
    /// Obtiene todos los resultados (partidos finalizados) de una competición
    /// </summary>
    [HttpGet("{competicionId:int}/resultados")]
    public async Task<IActionResult> ObtenerResultados(int competicionId)
    {
        try
        {
            var resultados = await _competicionService.ObtenerResultadosAsync(competicionId);
            return Ok(resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resultados");
            return StatusCode(500, new { message = "Error al obtener resultados" });
        }
    }

    /// <summary>
    /// Obtiene la clasificación de una competición
    /// </summary>
    [HttpGet("{competicionId:int}/clasificacion")]
    public async Task<IActionResult> ObtenerClasificacion(int competicionId)
    {
        try
        {
            var clasificacion = await _competicionService.ObtenerClasificacionAsync(competicionId);
            return Ok(clasificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clasificación");
            return StatusCode(500, new { message = "Error al obtener clasificación" });
        }
    }

    /// <summary>
    /// Obtiene los goleadores de una competición
    /// </summary>
    [HttpGet("{competicionId:int}/goleadores")]
    public async Task<IActionResult> ObtenerGoleadores(int competicionId)
    {
        try
        {
            var goleadores = await _competicionService.ObtenerGoleadoresAsync(competicionId);
            return Ok(goleadores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener goleadores");
            return StatusCode(500, new { message = "Error al obtener goleadores" });
        }
    }

    [HttpGet("ligas/{ligaId:int}/jornadas")]
    public async Task<IActionResult> ObtenerJornadasPorLiga(int ligaId)
    {
        try
        {
            var jornadas = await _ligaService.ObtenerJornadasAsync(ligaId);
            return Ok(jornadas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener jornadas por liga");
            return StatusCode(500, new { message = "Error al obtener jornadas por liga" });
        }
    }

    [HttpGet("ligas/{ligaId:int}/resultados")]
    public async Task<IActionResult> ObtenerResultadosPorLiga(int ligaId)
    {
        try
        {
            var resultados = await _ligaService.ObtenerResultadosAsync(ligaId);
            return Ok(resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resultados por liga");
            return StatusCode(500, new { message = "Error al obtener resultados por liga" });
        }
    }

    [HttpGet("ligas/{ligaId:int}/clasificacion")]
    public async Task<IActionResult> ObtenerClasificacionPorLiga(int ligaId)
    {
        try
        {
            var clasificacion = await _ligaService.ObtenerClasificacionAsync(ligaId);
            return Ok(clasificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clasificación por liga");
            return StatusCode(500, new { message = "Error al obtener clasificación por liga" });
        }
    }

    [HttpGet("ligas/{ligaId:int}/goleadores")]
    public async Task<IActionResult> ObtenerGoleadoresPorLiga(int ligaId)
    {
        try
        {
            var goleadores = await _ligaService.ObtenerGoleadoresAsync(ligaId);
            return Ok(goleadores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener goleadores por liga");
            return StatusCode(500, new { message = "Error al obtener goleadores por liga" });
        }
    }

    /// <summary>
    /// Busca competiciones con filtros
    /// </summary>
    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarCompeticiones(
        [FromQuery] string? temporada,
        [FromQuery] string? tipoJuego,
        [FromQuery] string? competicion,
        [FromQuery] string? grupo)
    {
        try
        {
            var jornadas = await _competicionService.BuscarCompeticionesAsync(temporada, tipoJuego, competicion, grupo);
            return Ok(jornadas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar competiciones");
            return StatusCode(500, new { message = "Error al buscar competiciones" });
        }
    }
}

