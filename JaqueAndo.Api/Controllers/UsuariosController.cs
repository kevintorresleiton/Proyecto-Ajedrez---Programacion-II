using JaqueAndo.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JaqueAndo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(AppDbContext db) : ControllerBase
{
    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { mensaje = "Ingresá usuario y contraseña." });

        var user = await db.Usuarios.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == req.Username.Trim().ToLower() && u.Password == req.Password);

        if (user is null)
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

        // No devolvemos la contraseña
        return Ok(new { user.Id, user.Username, user.NombreCompleto, user.EsAdmin });
    }
}