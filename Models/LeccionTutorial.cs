using System.Collections.Generic;

namespace JaqueAndo.Models;

// Describe UNA lección del tutorial.
// Es solo información: no tiene lógica, el "motor" está en TutorialViewModel.
public class LeccionTutorial
{
    // Título que se ve arriba del panel, por ejemplo "Lección 1: El peón"
    public string Titulo { get; set; } = string.Empty;

    // Explicación de la lección
    public string Texto { get; set; } = string.Empty;

    // Lo que tiene que lograr el usuario
    public string Objetivo { get; set; } = string.Empty;

    // Posición inicial del tablero en formato FEN
    public string Fen { get; set; } = string.Empty;

    // Jugadas que cumplen el objetivo, escritas como "origen + destino".
    // Ejemplo: "e2e4" es mover la pieza de e2 a e4.
    // Puede haber varias si la lección tiene más de una solución.
    public List<string> MovimientosCorrectos { get; set; } = new();

    // Mensaje cuando el usuario hace la jugada correcta
    public string MensajeAcierto { get; set; } = string.Empty;

    // Mensaje cuando hace una jugada válida pero que no es la que pide la lección
    public string MensajeError { get; set; } = string.Empty;

    // Una lección es informativa si no tiene jugadas correctas: solo se explora, no se juega.
    public bool EsInformativa => MovimientosCorrectos.Count == 0;
}