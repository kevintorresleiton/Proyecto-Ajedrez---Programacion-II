namespace JaqueAndo.Api.Models;

public class Torneo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public string Modalidad { get; set; } = "Rapid";
    public List<Inscripcion> Inscripciones { get; set; } = new();
}
