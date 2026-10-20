using System.ComponentModel.DataAnnotations;
namespace JaqueAndo.Api.Models;

public class Jugador
{
    public int Id { get; set; }
    [Required, StringLength(50)] public string Nombre { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Apellido { get; set; } = string.Empty;
    public string IdFide { get; set; } = "N/A";
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = "M";
    public bool EsSocio { get; set; }
    [Range(0, 3500)] public int EloEstimado { get; set; } = 1200;
    public int? ClubId { get; set; }
    public Club? Club { get; set; }
}
