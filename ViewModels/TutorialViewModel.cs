using System.Collections.ObjectModel;
using ChessDotNet;          // Librería de ajedrez: conoce las reglas y las piezas
using ChessDotNet.Pieces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;

namespace JaqueAndo.ViewModels;

// ViewModel de la pantalla del tutorial.
public partial class TutorialViewModel : ViewModelBase
{
    // ---------- Colores que usamos ----------
    private const string ColorNeutro = "#94A3B8";     // gris: mensajes normales
    private const string ColorAcierto = "#4ADE80";    // verde: respuesta correcta
    private const string ColorError = "#F87171";      // rojo: movimiento incorrecto
    private const string ColorSeleccion = "#F59E0B";  // naranja: pieza elegida
    private const string ColorDestino = "#86EFAC";    // verde claro: a dónde puede ir

    // ---------- Datos de la lección 1 (en el paso 5 los movemos a una clase aparte) ----------
    // Posición inicial en formato FEN: rey negro en e8, peón blanco en e2, rey blanco en e1
    private const string FenLeccion = "4k3/8/8/8/8/8/4P3/4K3 w - - 0 1";
    // Casilla que hay que alcanzar: e4
    private const File MetaColumna = File.E;
    private const int MetaFila = 4;

    // Referencia al ViewModel principal: la necesitamos para cambiar de pantalla
    private readonly MainViewModel _mainViewModel;

    // La partida de ChessDotNet: sabe dónde está cada pieza y qué movimientos son válidos
    private ChessGame _game = new ChessGame();

    // Casilla de la pieza que el usuario tocó primero (null = todavía no eligió ninguna)
    private CasillaTutorialViewModel? _casillaOrigen = null;

    // Usuario que inició sesión (se pasa de pantalla en pantalla)
    [ObservableProperty] private Usuario _usuarioActual;

    // Título de la pantalla
    [ObservableProperty] private string _titulo = "Tutorial de Ajedrez";

    // Textos del panel lateral
    [ObservableProperty] private string _tituloLeccion = "Lección 1: El peón";
    [ObservableProperty] private string _textoLeccion =
        "El peón avanza hacia adelante, nunca hacia atrás. Normalmente se mueve una casilla, " +
        "pero en su primer movimiento puede avanzar una o dos. Tocá el peón blanco para ver a dónde puede ir.";
    [ObservableProperty] private string _objetivo = "Objetivo: llevá el peón de e2 a e4.";

    // Mensaje de respuesta al usuario (acierto, error o ayuda) y su color
    [ObservableProperty] private string _mensaje = "Tocá el peón blanco para empezar.";
    [ObservableProperty] private string _colorMensaje = ColorNeutro;

    // true cuando el usuario ya cumplió el objetivo (se bloquea el tablero)
    [ObservableProperty] private bool _leccionCompletada = false;

    // Las 64 casillas del tablero
    [ObservableProperty] private ObservableCollection<CasillaTutorialViewModel> _tablero = new();

    public TutorialViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        CargarPosicion();
    }

    // Arma el tablero a partir del FEN de la lección
    private void CargarPosicion()
    {
        _game = new ChessGame(FenLeccion);   // partida con la posición de la lección
        _casillaOrigen = null;
        LeccionCompletada = false;
        Tablero.Clear();

        // r = 0 es la fila de arriba (la fila 8 del ajedrez)
        for (int r = 0; r < 8; r++)
        {
            int filaChess = 8 - r;

            // c = 0 es la columna de la izquierda (la "a")
            for (int c = 0; c < 8; c++)
            {
                File colChess = (File)c;
                Position pos = new Position(colChess, filaChess);
                Piece? piece = _game.GetPieceAt(pos);

                // Las casillas se alternan: clara, oscura, clara, oscura...
                bool esClara = (r + c) % 2 == 0;
                string color = esClara ? "#F0D9B5" : "#B58863";

                Tablero.Add(new CasillaTutorialViewModel
                {
                    FilaChess = filaChess,
                    ColumnaChess = colChess,
                    Pieza = ConvertirPiezaAUnicode(piece),
                    ColorBase = color,
                    ColorFondo = color
                });
            }
        }
    }

    // Convierte una pieza de ChessDotNet en su símbolo Unicode
    private string ConvertirPiezaAUnicode(Piece? piece)
    {
        if (piece == null) return string.Empty;   // casilla vacía

        return (piece.Owner, piece.GetType().Name) switch
        {
            (Player.White, nameof(Pawn)) => "♙",
            (Player.White, nameof(Knight)) => "♘",
            (Player.White, nameof(Bishop)) => "♗",
            (Player.White, nameof(Rook)) => "♖",
            (Player.White, nameof(Queen)) => "♕",
            (Player.White, nameof(King)) => "♔",
            (Player.Black, nameof(Pawn)) => "♟",
            (Player.Black, nameof(Knight)) => "♞",
            (Player.Black, nameof(Bishop)) => "♝",
            (Player.Black, nameof(Rook)) => "♜",
            (Player.Black, nameof(Queen)) => "♛",
            (Player.Black, nameof(King)) => "♚",
            _ => string.Empty
        };
    }

    // Se ejecuta cada vez que el usuario toca una casilla
    [RelayCommand]
    private void SeleccionarCasilla(CasillaTutorialViewModel casilla)
    {
        // Si la lección ya terminó, el tablero queda bloqueado
        if (LeccionCompletada) return;

        Position pos = new Position(casilla.ColumnaChess, casilla.FilaChess);
        Piece? piece = _game.GetPieceAt(pos);

        // CASO 1: todavía no hay pieza elegida -> este toque elige la pieza
        if (_casillaOrigen == null)
        {
            if (piece != null && piece.Owner == _game.WhoseTurn)
            {
                _casillaOrigen = casilla;
                casilla.ColorFondo = ColorSeleccion;   // naranja
                MostrarDestinosValidos(casilla);        // verde donde puede ir
                MostrarMensaje("Las casillas verdes son las jugadas posibles. Tocá la casilla a la que querés mover.", ColorNeutro);
            }
            else
            {
                MostrarMensaje("Tocá una pieza blanca para elegirla.", ColorError);
            }
            return;
        }

        // CASO 2: ya hay pieza elegida -> este toque es el destino

        // Si toca la misma pieza otra vez, cancelamos la selección
        if (casilla == _casillaOrigen)
        {
            LimpiarSeleccion();
            MostrarMensaje("Cancelaste la selección. Tocá una pieza blanca.", ColorNeutro);
            return;
        }

        Position posOrigen = new Position(_casillaOrigen.ColumnaChess, _casillaOrigen.FilaChess);
        Move move = new Move(posOrigen, pos, _game.WhoseTurn);

        if (_game.IsValidMove(move))
        {
            // El movimiento es legal: lo hacemos en la partida y actualizamos las piezas
            _game.MakeMove(move, true);
            LimpiarSeleccion();
            ActualizarPiezas();

            // ¿Llegó a la casilla objetivo?
            if (casilla.ColumnaChess == MetaColumna && casilla.FilaChess == MetaFila)
            {
                LeccionCompletada = true;
                MostrarMensaje("¡Muy bien! En su primer movimiento el peón avanzó dos casillas hasta e4.", ColorAcierto);
            }
            else
            {
                // Movimiento válido, pero no es el objetivo: reiniciamos para que pueda reintentar
                CargarPosicion();
                MostrarMensaje("Ese movimiento es válido, pero en esta lección el objetivo es llevar el peón a e4. ¡Probá de nuevo!", ColorError);
            }
        }
        else
        {
            // Movimiento ilegal: avisamos y limpiamos la selección
            LimpiarSeleccion();
            MostrarMensaje("Ese movimiento no es posible con esa pieza. Elegí la pieza y mirá las casillas verdes.", ColorError);
        }
    }

    // Pinta de verde las casillas a las que puede ir la pieza elegida.
    // Probamos las 64 casillas y le preguntamos a ChessDotNet cuáles son válidas.
    private void MostrarDestinosValidos(CasillaTutorialViewModel origen)
    {
        Position posOrigen = new Position(origen.ColumnaChess, origen.FilaChess);

        foreach (var casilla in Tablero)
        {
            if (casilla == origen) continue;   // la casilla de origen no cuenta

            Position posDestino = new Position(casilla.ColumnaChess, casilla.FilaChess);
            Move move = new Move(posOrigen, posDestino, _game.WhoseTurn);

            if (_game.IsValidMove(move))
            {
                casilla.ColorFondo = ColorDestino;
            }
        }
    }

    // Quita todos los resaltados y deja cada casilla con su color original
    private void LimpiarSeleccion()
    {
        foreach (var casilla in Tablero)
        {
            casilla.ColorFondo = casilla.ColorBase;
        }
        _casillaOrigen = null;
    }

    // Vuelve a leer la partida y actualiza el símbolo de cada casilla
    private void ActualizarPiezas()
    {
        foreach (var casilla in Tablero)
        {
            Position pos = new Position(casilla.ColumnaChess, casilla.FilaChess);
            casilla.Pieza = ConvertirPiezaAUnicode(_game.GetPieceAt(pos));
        }
    }

    // Cambia el texto y el color del mensaje del panel
    private void MostrarMensaje(string texto, string color)
    {
        Mensaje = texto;
        ColorMensaje = color;
    }

    // Botón "Reiniciar lección": vuelve a la posición inicial
    [RelayCommand]
    private void ReiniciarLeccion()
    {
        CargarPosicion();
        MostrarMensaje("Tocá el peón blanco para empezar.", ColorNeutro);
    }

    // Botón "Volver al Menú": navega de nuevo al menú principal
    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}