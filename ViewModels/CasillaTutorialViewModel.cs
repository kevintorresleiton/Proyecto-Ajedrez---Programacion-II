using ChessDotNet;
using CommunityToolkit.Mvvm.ComponentModel;

namespace JaqueAndo.ViewModels;

// Representa UNA casilla del tablero del tutorial.
// Es propia del tutorial para no depender de la casilla de la pantalla Jugar.
public partial class CasillaTutorialViewModel : ObservableObject
{
    // Posición de la casilla en el formato de ChessDotNet (columna a-h, fila 1-8).
    // Más adelante las vamos a usar para saber a qué casilla se tocó.
    public File ColumnaChess { get; set; }
    public int FilaChess { get; set; }

    // Símbolo de la pieza que hay en la casilla ("" si está vacía)
    [ObservableProperty] private string _pieza = string.Empty;

    // Color de fondo de la casilla (clara u oscura)
    [ObservableProperty] private string _colorFondo = "#F0D9B5";
}