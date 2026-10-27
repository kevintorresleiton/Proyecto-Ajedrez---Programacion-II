using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChessDotNet;          // Librería de ajedrez: conoce las reglas y las piezas
using ChessDotNet.Pieces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;

namespace JaqueAndo.ViewModels;

// ViewModel de la pantalla del tutorial. Es el "motor": sabe recorrer cualquier lección.
public partial class TutorialViewModel : ViewModelBase
{
    // ---------- Colores que usamos ----------
    private const string ColorNeutro = "#94A3B8";     // gris: mensajes normales
    private const string ColorAcierto = "#4ADE80";    // verde: respuesta correcta
    private const string ColorError = "#F87171";      // rojo: movimiento incorrecto
    private const string ColorSeleccion = "#F59E0B";  // naranja: pieza elegida
    private const string ColorDestino = "#86EFAC";    // verde claro: a dónde puede ir

    // Referencia al ViewModel principal: la necesitamos para cambiar de pantalla
    private readonly MainViewModel _mainViewModel;

    // La partida de ChessDotNet: sabe dónde está cada pieza y qué movimientos son válidos
    private ChessGame _game = new ChessGame();

    // Casilla de la pieza que el usuario tocó primero (null = todavía no eligió ninguna)
    private CasillaTutorialViewModel? _casillaOrigen = null;

    // Todas las lecciones (vienen de ContenidoTutorial)
    private readonly List<LeccionTutorial> _lecciones = ContenidoTutorial.ObtenerLecciones();

    // Número de la lección actual (0 = la primera) y la lección en sí
    private int _indiceLeccion = 0;
    private LeccionTutorial _leccionActual = new LeccionTutorial();

    // Usuario que inició sesión (se pasa de pantalla en pantalla)
    [ObservableProperty] private Usuario _usuarioActual;

    // Título de la pantalla
    [ObservableProperty] private string _titulo = "Tutorial de Ajedrez";

    // Textos del panel lateral (se completan al cargar cada lección)
    [ObservableProperty] private string _textoProgreso = string.Empty;   // "Lección 1 de 2"
    [ObservableProperty] private string _tituloLeccion = string.Empty;
    [ObservableProperty] private string _textoLeccion = string.Empty;
    [ObservableProperty] private string _objetivo = string.Empty;

    // Mensaje de respuesta al usuario (acierto, error o ayuda) y su color
    [ObservableProperty] private string _mensaje = string.Empty;
    [ObservableProperty] private string _colorMensaje = ColorNeutro;

    // true cuando el usuario ya cumplió el objetivo (se bloquea el tablero)
    [ObservableProperty] private bool _leccionCompletada = false;

    // Las 64 casillas del tablero
    [ObservableProperty] private ObservableCollection<CasillaTutorialViewModel> _tablero = new();

    public TutorialViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        CargarLeccion(0);   // empezamos por la primera lección
    }

    // Carga la lección número "indice": textos, tablero y mensaje inicial
    private void CargarLeccion(int indice)
    {
        _indiceLeccion = indice;
        _leccionActual = _lecciones[indice];

        // Pasamos los datos de la lección al panel
        TituloLeccion = _leccionActual.Titulo;
        TextoLeccion = _leccionActual.Texto;
        Objetivo = _leccionActual.Objetivo;
        TextoProgreso = $"Lección {indice + 1} de {_lecciones.Count}";

        CargarPosicion();
        // El mensaje inicial depende del tipo de lección
        if (_leccionActual.EsInformativa)
        {
            MostrarMensaje("Esta lección es para explorar. Tocá una pieza blanca para ver a dónde puede moverse.", ColorNeutro);
        }
        else
        {
            MostrarMensaje("Tocá una pieza blanca para empezar.", ColorNeutro);
        }

        // Avisamos a los botones Anterior / Siguiente que revisen si se pueden usar
        SiguienteLeccionCommand.NotifyCanExecuteChanged();
        LeccionAnteriorCommand.NotifyCanExecuteChanged();
    }

    // Arma el tablero a partir del FEN de la lección actual
    private void CargarPosicion()
    {
        _game = new ChessGame(_leccionActual.Fen);   // partida con la posición de la lección
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

    // Devuelve el nombre de una casilla en notación de ajedrez, por ejemplo "e4"
    private string NombreCasilla(CasillaTutorialViewModel casilla)
    {
        return casilla.ColumnaChess.ToString().ToLower() + casilla.FilaChess;
    }

    // Se ejecuta cada vez que el usuario toca una casilla
    [RelayCommand]
    private void SeleccionarCasilla(CasillaTutorialViewModel casilla)
    {
        // Si la lección ya terminó, el tablero queda bloqueado
        if (LeccionCompletada) return;
        
        // En las lecciones informativas solo se explora: no se hacen movimientos
        if (_leccionActual.EsInformativa)
        {
            ExplorarCasilla(casilla);
            return;
        }

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
            // Armamos el nombre de la jugada, por ejemplo "e2e4" (origen + destino).
            // Lo hacemos ANTES de limpiar la selección, porque ahí se borra el origen.
            string jugada = NombreCasilla(_casillaOrigen) + NombreCasilla(casilla);

            // El movimiento es legal: lo hacemos en la partida y actualizamos las piezas
            _game.MakeMove(move, true);
            LimpiarSeleccion();
            ActualizarPiezas();

            // ¿La jugada cumple el objetivo de la lección? (depende del tipo de meta)
            if (CumpleObjetivo(jugada))
            {
                LeccionCompletada = true;
                MostrarMensaje(_leccionActual.MensajeAcierto, ColorAcierto);
            }
            else
            {
                // Movimiento válido, pero no es el que pide la lección: reiniciamos para reintentar
                CargarPosicion();
                MostrarMensaje(_leccionActual.MensajeError, ColorError);
            }
        }
        else
        {
            // Movimiento ilegal: avisamos y limpiamos la selección
            LimpiarSeleccion();
            MostrarMensaje("Ese movimiento no es posible con esa pieza. Elegí la pieza y mirá las casillas verdes.", ColorError);
        }
    }
    // Decide si lo que hizo el usuario cumple el objetivo de la lección.
    // Se llama DESPUÉS de hacer la jugada, así que ahora le toca mover a las negras.
    private bool CumpleObjetivo(string jugada)
    {
        // Jugador que tiene que mover ahora (el rival de quien acaba de jugar)
        Player rival = _game.WhoseTurn;

        switch (_leccionActual.Meta)
        {
            case TipoMeta.Jaque:
                // ¿El rival quedó en jaque?
                return _game.IsInCheck(rival);

            case TipoMeta.JaqueMate:
                // ¿El rival quedó en jaque mate?
                return _game.IsCheckmated(rival);

            case TipoMeta.Ahogado:
                // Ahogado = NO está en jaque y NO tiene ninguna jugada legal
                return !_game.IsInCheck(rival) && !TieneJugadasLegales(rival);

            default:
                // Meta "Jugada": la jugada tiene que estar en la lista de la lección
                return _leccionActual.MovimientosCorrectos.Contains(jugada);
        }
    }

    // Devuelve true si el jugador tiene al menos una jugada legal.
    // Prueba cada pieza suya contra cada casilla del tablero.
    private bool TieneJugadasLegales(Player jugador)
    {
        foreach (var origen in Tablero)
        {
            Position posOrigen = new Position(origen.ColumnaChess, origen.FilaChess);
            Piece? piece = _game.GetPieceAt(posOrigen);

            // Nos interesan solo las piezas de este jugador
            if (piece == null || piece.Owner != jugador) continue;

            foreach (var destino in Tablero)
            {
                if (destino == origen) continue;   // no se puede quedar en el lugar

                Position posDestino = new Position(destino.ColumnaChess, destino.FilaChess);
                Move move = new Move(posOrigen, posDestino, jugador);

                // Con encontrar UNA jugada legal ya alcanza
                if (_game.IsValidMove(move)) return true;
            }
        }

        return false;   // ninguna pieza tiene jugadas legales
    }
    // Modo exploración (lecciones informativas): al tocar una pieza blanca se muestran
    // sus jugadas posibles, pero no se mueve nada.
    private void ExplorarCasilla(CasillaTutorialViewModel casilla)
    {
        LimpiarSeleccion();   // borramos lo que se había resaltado antes

        Position pos = new Position(casilla.ColumnaChess, casilla.FilaChess);
        Piece? piece = _game.GetPieceAt(pos);

        if (piece != null && piece.Owner == _game.WhoseTurn)
        {
            casilla.ColorFondo = ColorSeleccion;   // naranja: la pieza que tocó
            MostrarDestinosValidos(casilla);        // verde: a dónde puede ir
            MostrarMensaje("Las casillas verdes son las jugadas posibles de esa pieza. Probá con otra.", ColorNeutro);
        }
        else
        {
            MostrarMensaje("Tocá una pieza blanca para ver sus jugadas.", ColorNeutro);
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

    // ¿Hay una lección después de la actual? (para habilitar el botón "Siguiente")
    private bool HaySiguiente() => _indiceLeccion < _lecciones.Count - 1;

    // ¿Hay una lección antes de la actual? (para habilitar el botón "Anterior")
    private bool HayAnterior() => _indiceLeccion > 0;

    // Botón "Siguiente →"
    [RelayCommand(CanExecute = nameof(HaySiguiente))]
    private void SiguienteLeccion() => CargarLeccion(_indiceLeccion + 1);

    // Botón "← Anterior"
    [RelayCommand(CanExecute = nameof(HayAnterior))]
    private void LeccionAnterior() => CargarLeccion(_indiceLeccion - 1);

    // Botón "Reiniciar lección": vuelve a cargar la lección actual desde cero
    [RelayCommand]
    private void ReiniciarLeccion() => CargarLeccion(_indiceLeccion);

    // Botón "Volver al Menú": navega de nuevo al menú principal
    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}