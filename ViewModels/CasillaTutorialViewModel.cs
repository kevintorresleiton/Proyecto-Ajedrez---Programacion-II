using ChessDotNet;
using CommunityToolkit.Mvvm.ComponentModel;

namespace JaqueAndo.ViewModels;

// Representa UNA casilla del tablero del tutorial.
public partial class CasillaTutorialViewModel : ObservableObject
{
    // Posición de la casilla en el formato de ChessDotNet (columna a-h, fila 1-8)
    public File ColumnaChess { get; set; }
    public int FilaChess { get; set; }

    // Color original de la casilla (clara u oscura).
    // Lo guardamos para poder restaurarlo después de resaltarla.
    public string ColorBase { get; set; } = "#F0D9B5";

    // Símbolo de la pieza que hay en la casilla ("" si está vacía)
    [ObservableProperty] private string _pieza = string.Empty;

    // Color que se ve ahora: puede ser el base, naranja (seleccionada) o verde (destino posible)
    [ObservableProperty] private string _colorFondo = "#F0D9B5";
}