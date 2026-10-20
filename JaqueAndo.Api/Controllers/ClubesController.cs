using JaqueAndo.Api.Data;
using JaqueAndo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Clubes
            .Select(c => new { c.Id, c.Nombre, c.Ciudad, CantidadJugadores = c.Jugadores.Count })
            .OrderBy(c => c.Nombre)
            .ToListAsync());

    [HttpPost]
    public async Task<ActionResult<Club>> Create(Club club)
    {
        club.Id = 0;
        club.Jugadores = new();
        db.Clubes.Add(club);
        await db.SaveChangesAsync();
        return Created($"api/clubes/{club.Id}", club);
    }
}