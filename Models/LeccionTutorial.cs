using System.Collections.Generic;

namespace JaqueAndo.Models;

// Qué tiene que lograr el usuario en una lección
public enum TipoMeta
{
    Jugada,      // hacer una de las jugadas de la lista MovimientosCorrectos (lo habitual)
    Jaque,       // dar jaque al rey negro
    JaqueMate,   // dar jaque mate al rey negro
    Ahogado      // dejar al rey negro ahogado (sin jugadas legales y sin estar en jaque)
}

// Describe UNA lección del tutorial.
// Es solo información: no tiene lógica, el "motor" está en TutorialViewModel.
public class LeccionTutorial
{
    // Título que se ve arriba del panel, por ejemplo "El peón"
    public string Titulo { get; set; } = string.Empty;

    // Explicación de la lección
    public string Texto { get; set; } = string.Empty;

    // Lo que tiene que lograr el usuario
    public string Objetivo { get; set; } = string.Empty;

    // Posición inicial del tablero en formato FEN
    public string Fen { get; set; } = string.Empty;

    // Tipo de meta. Si no se indica, es "Jugada".
    public TipoMeta Meta { get; set; } = TipoMeta.Jugada;

    // Jugadas que cumplen el objetivo, escritas como "origen + destino" (ejemplo: "e2e4").
    // Solo se usa cuando la meta es Jugada.
    public List<string> MovimientosCorrectos { get; set; } = new();

    // Mensaje cuando el usuario cumple el objetivo
    public string MensajeAcierto { get; set; } = string.Empty;

    // Mensaje cuando hace una jugada válida pero que no cumple el objetivo
    public string MensajeError { get; set; } = string.Empty;

    // Una lección es informativa si su meta es "Jugada" pero no tiene jugadas correctas:
    // solo se explora, no se juega.
    public bool EsInformativa => Meta == TipoMeta.Jugada && MovimientosCorrectos.Count == 0;
}