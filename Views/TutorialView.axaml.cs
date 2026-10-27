//el archivo de apoyo que toda vista de Avalonia necesita (es idéntico al de JugarView).
using Avalonia.Controls;

namespace JaqueAndo.Views;

// Code-behind de la vista del tutorial.
// InitializeComponent carga el diseño que escribimos en el .axaml
public partial class TutorialView : UserControl
{
    public TutorialView()
    {
        InitializeComponent();
    }
}