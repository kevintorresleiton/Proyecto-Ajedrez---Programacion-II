using System.Collections.Generic;

namespace JaqueAndo.Models;

// Acá viven TODAS las lecciones del tutorial.
// Para agregar una lección nueva solo hay que sumar un bloque "new LeccionTutorial { ... }".
public static class ContenidoTutorial
{
    public static List<LeccionTutorial> ObtenerLecciones()
    {
        return new List<LeccionTutorial>
        {
            // ---------- Lección 1: el peón ----------
            new LeccionTutorial
            {
                Titulo = "Lección 1: El peón",
                Texto = "El peón avanza hacia adelante, nunca hacia atrás. Normalmente se mueve una casilla, " +
                        "pero en su primer movimiento puede avanzar una o dos. Tocá el peón blanco para ver a dónde puede ir.",
                Objetivo = "Objetivo: llevá el peón de e2 a e4.",
                // Rey negro en e8, peón blanco en e2, rey blanco en e1 (juegan las blancas)
                Fen = "4k3/8/8/8/8/8/4P3/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "e2e4" },
                MensajeAcierto = "¡Muy bien! En su primer movimiento el peón avanzó dos casillas hasta e4.",
                MensajeError = "Ese movimiento es válido, pero en esta lección el objetivo es llevar el peón a e4. ¡Probá de nuevo!"
            },

            // ---------- Lección 2: la torre ----------
            new LeccionTutorial
            {
                Titulo = "Lección 2: La torre",
                Texto = "La torre se mueve en línea recta, en horizontal o en vertical, todas las casillas que quiera " +
                        "mientras no haya piezas en el camino. Tocá la torre blanca para ver sus jugadas.",
                Objetivo = "Objetivo: llevá la torre de d4 a h4.",
                // Rey negro en e8, torre blanca en d4, rey blanco en e1 (juegan las blancas)
                Fen = "4k3/8/8/8/3R4/8/8/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "d4h4" },
                MensajeAcierto = "¡Excelente! La torre recorrió el tablero en horizontal hasta h4, pasando por e4, f4 y g4 porque estaban libres.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es llevar la torre de d4 a h4. ¡Probá de nuevo!"
            }
        };
    }
}