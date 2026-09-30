namespace JaqueAndo.Models;

public class EventoLichess
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty; // Blitz, Rapid, Classical
    public int Rondas { get; set; }
    public string Estado { get; set; } = string.Empty;
}