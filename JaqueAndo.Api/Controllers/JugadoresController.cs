using JaqueAndo.Api.Data;
using JaqueAndo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JugadoresController(AppDbContext db) : ControllerBase
{
    // GET api/jugadores?clubId=1&eloMinimo=1500
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jugador>>> GetAll(int? clubId, int? eloMinimo)
    {
        var query = db.Jugadores.Include(j => j.Club).AsQueryable();

        if (clubId.HasValue)
            query = query.Where(j => j.ClubId == clubId);
        if (eloMinimo.HasValue)
            query = query.Where(j => j.EloEstimado >= eloMinimo);

        return Ok(await query.OrderByDescending(j => j.EloEstimado).ToListAsync());
    }

    // GET api/jugadores/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Jugador>> GetById(int id)
    {
        var jugador = await db.Jugadores.Include(j => j.Club)
            .FirstOrDefaultAsync(j => j.Id == id);
        return jugador is null ? NotFound(new { mensaje = $"No existe el jugador {id}" }) : Ok(jugador);
    }

    // POST api/jugadores
    [HttpPost]
    public async Task<ActionResult<Jugador>> Create(Jugador jugador)
    {
        jugador.Id = 0;
        db.Jugadores.Add(jugador);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = jugador.Id }, jugador);
    }

    // PUT api/jugadores/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Jugador jugador)
    {
        if (id != jugador.Id)
            return BadRequest(new { mensaje = "El id de la URL no coincide con el del cuerpo" });
        if (!await db.Jugadores.AnyAsync(j => j.Id == id))
            return NotFound(new { mensaje = $"No existe el jugador {id}" });

        db.Entry(jugador).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/jugadores/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var jugador = await db.Jugadores.FindAsync(id);
        if (jugador is null)
            return NotFound(new { mensaje = $"No existe el jugador {id}" });

        db.Jugadores.Remove(jugador);
        await db.SaveChangesAsync();
        return NoContent();
    }
}