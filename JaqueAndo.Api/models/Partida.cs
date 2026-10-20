namespace JaqueAndo.Api.Models;

public class Partida
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int JugadorBlancasId { get; set; }
    public Jugador? JugadorBlancas { get; set; }
    public int JugadorNegrasId { get; set; }
    public Jugador? JugadorNegras { get; set; }
    public string Resultado { get; set; } = "*";
    public string Pgn { get; set; } = string.Empty;
}
