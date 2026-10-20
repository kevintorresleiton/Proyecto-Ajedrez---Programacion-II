using JaqueAndo.Api.Data;
using JaqueAndo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultadosTriviaController(AppDbContext db) : ControllerBase
{
    public record NuevoResultado(int UsuarioId, int Puntaje);

    [HttpPost]
    public async Task<IActionResult> Create(NuevoResultado dto)
    {
        if (dto.Puntaje < 0)
            return BadRequest(new { mensaje = "El puntaje no puede ser negativo." });
        if (!await db.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId))
            return NotFound(new { mensaje = $"No existe el usuario {dto.UsuarioId}." });

        var resultado = new ResultadoTrivia { UsuarioId = dto.UsuarioId, Puntaje = dto.Puntaje };
        db.ResultadosTrivia.Add(resultado);
        await db.SaveChangesAsync();
        return Created($"api/resultadostrivia/{resultado.Id}", new { resultado.Id, resultado.Puntaje, resultado.Fecha });
    }

    // GET api/resultadostrivia/ranking  -> top 10 por mejor puntaje
    [HttpGet("ranking")]
    public async Task<IActionResult> Ranking()
    {
        var ranking = await db.ResultadosTrivia
            .GroupBy(r => new { r.UsuarioId, r.Usuario!.NombreCompleto })
            .Select(g => new
            {
                g.Key.UsuarioId,
                Nombre = g.Key.NombreCompleto,
                MejorPuntaje = g.Max(r => r.Puntaje),
                Intentos = g.Count()
            })
            .OrderByDescending(x => x.MejorPuntaje)
            .Take(10)
            .ToListAsync();

        return Ok(ranking);
    }
}
