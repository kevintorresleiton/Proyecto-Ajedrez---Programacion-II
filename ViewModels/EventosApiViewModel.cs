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

    [ObservableProperty] private Usuario _usuarioActual;
    [ObservableProperty] private ObservableCollection<LichessTorneoDto> _torneos = new();
    [ObservableProperty] private ObservableCollection<LichessUserDto> _topMundo = new();

    [ObservableProperty] private string _busquedaId = string.Empty;
    [ObservableProperty] private LichessUserDto? _usuarioBuscado;
    [ObservableProperty] private bool _tieneUsuarioBuscado = false;
    [ObservableProperty] private string _mensajeBusqueda = string.Empty;
    [ObservableProperty] private bool _cargando = false;

    public EventosApiViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;
        _ = CargarTodoAsync();
    }

    [RelayCommand]
    private async Task CargarTodoAsync()
    {
        Cargando = true;

        var taskTorneos = ChessApiService.ObtenerTorneosOficialesAsync();
        var taskTopMundo = ChessApiService.ObtenerTopMundialAsync();

        await Task.WhenAll(taskTorneos, taskTopMundo);

        Torneos = new ObservableCollection<LichessTorneoDto>(taskTorneos.Result);
        TopMundo = new ObservableCollection<LichessUserDto>(taskTopMundo.Result);

        Cargando = false;
    }

    [RelayCommand]
    private async Task BuscarJugadorAsync()
    {
        if (string.IsNullOrWhiteSpace(BusquedaId)) return;

        Cargando = true;
        MensajeBusqueda = string.Empty;
        UsuarioBuscado = null;
        TieneUsuarioBuscado = false;

        var user = await ChessApiService.BuscarUsuarioAsync(BusquedaId);

        if (user != null)
        {
            UsuarioBuscado = user;
            TieneUsuarioBuscado = true;
        }
        else
        {
            MensajeBusqueda = "❌ Jugador no encontrado en Lichess.";
        }

        Cargando = false;
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}