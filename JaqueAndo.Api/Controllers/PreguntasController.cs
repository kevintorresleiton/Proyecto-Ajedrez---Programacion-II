using JaqueAndo.Api.Data;
using JaqueAndo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreguntasController(AppDbContext db) : ControllerBase
{
    // GET api/preguntas?cantidad=5  (en orden aleatorio)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PreguntaTrivia>>> Get(int cantidad = 5)
    {
        if (cantidad < 1 || cantidad > 50)
            return BadRequest(new { mensaje = "La cantidad debe estar entre 1 y 50." });

        return Ok(await db.Preguntas.OrderBy(p => EF.Functions.Random()).Take(cantidad).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<PreguntaTrivia>> Create(PreguntaTrivia pregunta)
    {
        if (pregunta.OpcionCorrecta < 1 || pregunta.OpcionCorrecta > 4)
            return BadRequest(new { mensaje = "OpcionCorrecta debe ser 1, 2, 3 o 4." });

        pregunta.Id = 0;
        db.Preguntas.Add(pregunta);
        await db.SaveChangesAsync();
        return Created($"api/preguntas/{pregunta.Id}", pregunta);
    }
}