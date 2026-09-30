using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using ChessDotNet; // Librería de Ajedrez
using ChessDotNet.Pieces;

namespace JaqueAndo.ViewModels;

public partial class CasillaViewModel : ObservableObject
{
    public int Fila { get; set; }
    public int Columna { get; set; }
    public File ColumnaChess { get; set; }
    public int FilaChess { get; set; }

    [ObservableProperty] private string _pieza = string.Empty;
    [ObservableProperty] private string _colorFondo = "#F0D9B5";
    [ObservableProperty] private bool _estaSeleccionada = false;
}

public partial class JugarViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private DispatcherTimer _timer;
    private ChessGame _game = new ChessGame();
    private CasillaViewModel? _casillaOrigen = null;

    [ObservableProperty] private Usuario _usuarioActual;
    [ObservableProperty] private ObservableCollection<CasillaViewModel> _tablero = new();
    [ObservableProperty] private string _turnoActual = "Blancas";
    [ObservableProperty] private string _tiempoBlancas = "05:00";
    [ObservableProperty] private string _tiempoNegras = "05:00";
    [ObservableProperty] private bool _juegoEnPausa = true;
    [ObservableProperty] private string _estadoJuego = "Partida Lista";

    private int _segundosBlancas = 300;
    private int _segundosNegras = 300;

    public JugarViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;

        InicializarTablero();
    }

    private void InicializarTablero()
    {
        _game = new ChessGame();
        Tablero.Clear();

        for (int r = 0; r < 8; r++)
        {
            int filaChess = 8 - r;
            for (int c = 0; c < 8; c++)
            {
                File colChess = (File)c;
                Position pos = new Position(colChess, filaChess);
                Piece? piece = _game.GetPieceAt(pos);

                bool esClara = (r + c) % 2 == 0;
                Tablero.Add(new CasillaViewModel
                {
                    Fila = r,
                    Columna = c,
                    FilaChess = filaChess,
                    ColumnaChess = colChess,
                    Pieza = ConvertirPiezaAUnicode(piece),
                    ColorFondo = esClara ? "#F0D9B5" : "#B58863"
                });
            }
        }
    }

    private string ConvertirPiezaAUnicode(Piece? piece)
    {
        if (piece == null) return string.Empty;

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

    [RelayCommand]
    private void SeleccionarCasilla(CasillaViewModel casilla)
    {
        if (JuegoEnPausa) return;

        if (_casillaOrigen == null)
        {
            // Selección de casilla de origen
            Position pos = new Position(casilla.ColumnaChess, casilla.FilaChess);
            Piece? piece = _game.GetPieceAt(pos);

            if (piece != null && piece.Owner == _game.WhoseTurn)
            {
                _casillaOrigen = casilla;
                casilla.EstaSeleccionada = true;
                casilla.ColorFondo = "#F59E0B"; // Color resalte selección
            }
        }
        else
        {
            // Intento de movimiento mediante ChessDotNet
            Position posOrigen = new Position(_casillaOrigen.ColumnaChess, _casillaOrigen.FilaChess);
            Position posDestino = new Position(casilla.ColumnaChess, casilla.FilaChess);

            Move move = new Move(posOrigen, posDestino, _game.WhoseTurn);

            if (_game.IsValidMove(move))
            {
                _game.MakeMove(move, true);
                ActualizarTableroVisual();
                ActualizarTurnoYEstado();
            }

            // Desmarcar origen
            bool esClara = (_casillaOrigen.Fila + _casillaOrigen.Columna) % 2 == 0;
            _casillaOrigen.ColorFondo = esClara ? "#F0D9B5" : "#B58863";
            _casillaOrigen.EstaSeleccionada = false;
            _casillaOrigen = null;
        }
    }

    private void ActualizarTableroVisual()
    {
        foreach (var casilla in Tablero)
        {
            Position pos = new Position(casilla.ColumnaChess, casilla.FilaChess);
            Piece? piece = _game.GetPieceAt(pos);
            casilla.Pieza = ConvertirPiezaAUnicode(piece);
        }
    }

    private void ActualizarTurnoYEstado()
    {
        TurnoActual = _game.WhoseTurn == Player.White ? "Blancas" : "Negras";

        if (_game.IsCheckmated(_game.WhoseTurn))
        {
            EstadoJuego = $"¡JAQUE MATE! Ganaron las {(TurnoActual == "Blancas" ? "Negras" : "Blancas")}";
            _timer.Stop();
            JuegoEnPausa = true;
        }
        else if (_game.IsInCheck(_game.WhoseTurn))
        {
            EstadoJuego = "¡JAQUE!";
        }
        else
        {
            EstadoJuego = "En Juego";
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (JuegoEnPausa) return;

        if (_game.WhoseTurn == Player.White)
        {
            if (_segundosBlancas > 0) _segundosBlancas--;
            TiempoBlancas = TimeSpan.FromSeconds(_segundosBlancas).ToString(@"mm\:ss");
        }
        else
        {
            if (_segundosNegras > 0) _segundosNegras--;
            TiempoNegras = TimeSpan.FromSeconds(_segundosNegras).ToString(@"mm\:ss");
        }
    }

    [RelayCommand]
    private void IniciarPausarJuego()
    {
        JuegoEnPausa = !JuegoEnPausa;
        if (JuegoEnPausa) _timer.Stop();
        else _timer.Start();
    }

    [RelayCommand]
    private void ReiniciarPartida()
    {
        _timer.Stop();
        _segundosBlancas = 300;
        _segundosNegras = 300;
        TiempoBlancas = "05:00";
        TiempoNegras = "05:00";
        TurnoActual = "Blancas";
        EstadoJuego = "Partida Lista";
        JuegoEnPausa = true;
        _casillaOrigen = null;
        InicializarTablero();
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _timer.Stop();
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}