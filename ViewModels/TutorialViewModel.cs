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
    // Referencia al ViewModel principal: la necesitamos para cambiar de pantalla
    private readonly MainViewModel _mainViewModel;

    // La partida de ChessDotNet: sabe dónde está cada pieza
    private ChessGame _game = new ChessGame();

    // Usuario que inició sesión (se pasa de pantalla en pantalla)
    [ObservableProperty] private Usuario _usuarioActual;

    [ObservableProperty] private string _titulo = "Tutorial de Ajedrez";

    // Las 64 casillas del tablero (la pantalla las dibuja en una grilla de 8x8)
    [ObservableProperty] private ObservableCollection<CasillaTutorialViewModel> _tablero = new();

    public TutorialViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        InicializarTablero();
    }

    // Arma las 64 casillas leyendo la partida de ChessDotNet
    private void InicializarTablero()
    {
        _game = new ChessGame();   // partida nueva, con las piezas en posición inicial
        Tablero.Clear();

        // r = 0 es la fila de arriba del tablero (la fila 8 del ajedrez)
        for (int r = 0; r < 8; r++)
        {
            int filaChess = 8 - r;

            // c = 0 es la columna de la izquierda (la columna "a")
            for (int c = 0; c < 8; c++)
            {
                File colChess = (File)c;
                Position pos = new Position(colChess, filaChess);
                Piece? piece = _game.GetPieceAt(pos);   // pieza en esa casilla (o null)

                // Las casillas se alternan: clara, oscura, clara, oscura...
                bool esClara = (r + c) % 2 == 0;

                Tablero.Add(new CasillaTutorialViewModel
                {
                    FilaChess = filaChess,
                    ColumnaChess = colChess,
                    Pieza = ConvertirPiezaAUnicode(piece),
                    ColorFondo = esClara ? "#F0D9B5" : "#B58863"
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

    // Botón "Volver al Menú": navega de nuevo al menú principal
    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}