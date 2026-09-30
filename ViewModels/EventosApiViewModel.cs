using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using JaqueAndo.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace JaqueAndo.ViewModels;

public partial class EventosApiViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private Usuario _usuarioActual;

    [ObservableProperty]
    private ObservableCollection<LichessTorneoDto> _torneos = new();

    [ObservableProperty]
    private bool _cargando = false;

    public EventosApiViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;
        _ = CargarEventosAsync();
    }

    [RelayCommand]
    private async Task CargarEventosAsync()
    {
        Cargando = true;
        var lista = await ChessApiService.ObtenerTorneosOficialesAsync();
        Torneos = new ObservableCollection<LichessTorneoDto>(lista);
        Cargando = false;
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}