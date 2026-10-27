using System.Collections.Generic;

namespace JaqueAndo.Models;

// Acá viven TODAS las lecciones del tutorial.
// Para agregar una lección nueva solo hay que sumar un bloque "new LeccionTutorial { ... }".
// Los títulos van sin número: el número lo muestra el contador del panel.
public static class ContenidoTutorial
{
    public static List<LeccionTutorial> ObtenerLecciones()
    {
        return new List<LeccionTutorial>
        {
            // ---------- El tablero (informativa: no tiene jugadas correctas) ----------
            new LeccionTutorial
            {
                Titulo = "El tablero y las piezas",
                Texto = "El tablero tiene 64 casillas: 8 columnas (de la a a la h) y 8 filas (del 1 al 8). " +
                        "Cada casilla se nombra con su columna y su fila, por ejemplo e4. " +
                        "Las blancas empiezan abajo y las negras arriba. La casilla de la esquina inferior derecha siempre es clara. " +
                        "Cada bando tiene un rey, una dama, dos torres, dos alfiles, dos caballos y ocho peones.",
                Objetivo = "Objetivo: conocer el tablero y las piezas.",
                // Posición inicial completa de una partida
                Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
                // Sin MovimientosCorrectos: por eso es una lección informativa
            },

            // ---------- El peón ----------
            new LeccionTutorial
            {
                Titulo = "El peón",
                Texto = "El peón avanza hacia adelante, nunca hacia atrás. Normalmente se mueve una casilla, " +
                        "pero en su primer movimiento puede avanzar una o dos. Tocá el peón blanco para ver a dónde puede ir.",
                Objetivo = "Objetivo: llevá el peón de e2 a e4.",
                // Rey negro en e8, peón blanco en e2, rey blanco en e1
                Fen = "4k3/8/8/8/8/8/4P3/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "e2e4" },
                MensajeAcierto = "¡Muy bien! En su primer movimiento el peón avanzó dos casillas hasta e4.",
                MensajeError = "Ese movimiento es válido, pero en esta lección el objetivo es llevar el peón a e4. ¡Probá de nuevo!"
            },

            // ---------- La torre ----------
            new LeccionTutorial
            {
                Titulo = "La torre",
                Texto = "La torre se mueve en línea recta, en horizontal o en vertical, todas las casillas que quiera " +
                        "mientras no haya piezas en el camino. Tocá la torre blanca para ver sus jugadas.",
                Objetivo = "Objetivo: llevá la torre de d4 a h4.",
                // Rey negro en e8, torre blanca en d4, rey blanco en e1
                Fen = "4k3/8/8/8/3R4/8/8/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "d4h4" },
                MensajeAcierto = "¡Excelente! La torre recorrió el tablero en horizontal hasta h4, pasando por e4, f4 y g4 porque estaban libres.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es llevar la torre de d4 a h4. ¡Probá de nuevo!"
            },

            // ---------- El alfil ----------
            new LeccionTutorial
            {
                Titulo = "El alfil",
                Texto = "El alfil se mueve en diagonal, todas las casillas que quiera mientras no haya piezas en el camino. " +
                        "Como siempre va en diagonal, cada alfil se queda toda la partida en casillas del mismo color. " +
                        "Tocá el alfil blanco para ver sus jugadas.",
                Objetivo = "Objetivo: llevá el alfil de c1 a h6.",
                // Rey negro en e8, alfil blanco en c1, rey blanco en e1
                Fen = "4k3/8/8/8/8/8/8/2B1K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "c1h6" },
                MensajeAcierto = "¡Muy bien! El alfil cruzó todo el tablero en diagonal, de c1 a h6.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es llevar el alfil de c1 a h6. ¡Probá de nuevo!"
            },

            // ---------- El caballo ----------
            new LeccionTutorial
            {
                Titulo = "El caballo",
                Texto = "El caballo se mueve en forma de L: dos casillas en una dirección y una más hacia un costado. " +
                        "Es la única pieza que puede saltar por encima de otras. Tocá el caballo blanco para ver sus jugadas.",
                Objetivo = "Objetivo: llevá el caballo de g1 a f3.",
                // Rey negro en e8, peones blancos en f2, g2 y h2, rey blanco en e1, caballo blanco en g1
                Fen = "4k3/8/8/8/8/8/5PPP/4K1N1 w - - 0 1",
                MovimientosCorrectos = new List<string> { "g1f3" },
                MensajeAcierto = "¡Perfecto! El caballo saltó por encima de sus peones y llegó a f3.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es llevar el caballo de g1 a f3. ¡Probá de nuevo!"
            },

            // ---------- La dama ----------
            new LeccionTutorial
            {
                Titulo = "La dama",
                Texto = "La dama es la pieza más poderosa: se mueve como una torre y como un alfil a la vez, " +
                        "en línea recta o en diagonal, todas las casillas que quiera. Tocá la dama blanca para ver sus jugadas.",
                Objetivo = "Objetivo: llevá la dama de d4 a g7.",
                // Rey negro en e8, dama blanca en d4, rey blanco en e1
                Fen = "4k3/8/8/8/3Q4/8/8/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "d4g7" },
                MensajeAcierto = "¡Muy bien! La dama llegó a g7 en diagonal. También podía haber ido en línea recta.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es llevar la dama de d4 a g7. ¡Probá de nuevo!"
            },

            // ---------- El rey ----------
            new LeccionTutorial
            {
                Titulo = "El rey",
                Texto = "El rey se mueve una sola casilla en cualquier dirección, pero nunca puede ir a una casilla " +
                        "que esté atacada por una pieza rival. Mirá la torre negra: controla toda la fila 5. " +
                        "Tocá el rey blanco y fijate qué casillas no aparecen en verde.",
                Objetivo = "Objetivo: llevá el rey de e4 a e3.",
                // Rey negro en e8, torre negra en h5, rey blanco en e4
                Fen = "4k3/8/8/7r/4K3/8/8/8 w - - 0 1",
                MovimientosCorrectos = new List<string> { "e4e3" },
                MensajeAcierto = "¡Muy bien! El rey avanzó a e3, una casilla que la torre negra no controla.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es llevar el rey de e4 a e3. ¡Probá de nuevo!"
            },

            // ---------- Las capturas ----------
            new LeccionTutorial
            {
                Titulo = "Las capturas",
                Texto = "Para capturar, se mueve una pieza a una casilla ocupada por una pieza rival, y esa pieza sale del tablero. " +
                        "La mayoría de las piezas capturan de la misma forma en que se mueven. " +
                        "Tocá la torre blanca: la casilla del caballo negro aparece en verde.",
                Objetivo = "Objetivo: capturá el caballo negro de g4 con la torre.",
                // Rey negro en e8, torre blanca en d4, caballo negro en g4, rey blanco en e1
                Fen = "4k3/8/8/8/3R2n1/8/8/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "d4g4" },
                MensajeAcierto = "¡Capturaste el caballo! La torre ocupó su casilla y el caballo salió del tablero.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es capturar el caballo negro de g4. ¡Probá de nuevo!"
            },

            // ---------- La captura del peón ----------
            new LeccionTutorial
            {
                Titulo = "La captura del peón",
                Texto = "El peón es la excepción: avanza en línea recta, pero captura en diagonal, una casilla hacia adelante. " +
                        "Por eso un peón nunca puede capturar a una pieza que tiene justo enfrente. " +
                        "Tocá el peón blanco y mirá qué casillas aparecen en verde.",
                Objetivo = "Objetivo: capturá el caballo negro de d5 con el peón.",
                // Rey negro en e8, caballo negro en d5, peón blanco en e4, rey blanco en e1
                Fen = "4k3/8/8/3n4/4P3/8/8/4K3 w - - 0 1",
                MovimientosCorrectos = new List<string> { "e4d5" },
                MensajeAcierto = "¡Muy bien! El peón capturó en diagonal: de e4 pasó a d5.",
                MensajeError = "Ese movimiento es válido, pero el objetivo es capturar el caballo negro de d5 con el peón. ¡Probá de nuevo!"
            }
        };
    }
}