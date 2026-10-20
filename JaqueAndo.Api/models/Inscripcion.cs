namespace JaqueAndo.Api.Models;

public class Inscripcion
{
    public int TorneoId { get; set; }
    public Torneo? Torneo { get; set; }
    public int JugadorId { get; set; }
    public Jugador? Jugador { get; set; }
    public double Puntos { get; set; }
}