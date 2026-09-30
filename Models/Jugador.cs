using System;

namespace JaqueAndo.Models;

public class Jugador
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string IdFide { get; set; } = "N/A";
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = "M"; // "M" o "F"
    public bool EsSocio { get; set; }
    public string Club { get; set; } = "Ninguno";
    public int EloEstimado { get; set; } = 1200;
}