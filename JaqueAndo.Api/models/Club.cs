using System.ComponentModel.DataAnnotations;
namespace JaqueAndo.Api.Models;

public class Club
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public List<Jugador> Jugadores { get; set; } = new();
}
